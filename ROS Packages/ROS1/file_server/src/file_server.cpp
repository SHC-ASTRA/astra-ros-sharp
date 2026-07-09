/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

- Added path traversal and extension checks to prevent unauthorized file access.
- Added parameters to enable/disable saving and overwriting files (disabled by default).
- Removed auto package generation for security.
    (C) Siemens AG, 2026, Mehmet Emre Cakal (emre.cakal@siemens.com/m.emrecakal@gmail.com)
*/

#include <ros/ros.h>
#include <ros/package.h>
#include <file_server/GetBinaryFile.h>
#include <file_server/SaveBinaryFile.h>
#include <iostream>
#include <fstream>
#include <filesystem>
#include <unordered_set>
#include <algorithm>

const std::unordered_set<std::string>& allowed_extensions()
{
    static const std::unordered_set<std::string> exts = {
        ".urdf", ".xacro", ".stl", ".dae", ".obj", ".mtl",
        ".png",  ".jpg",   ".jpeg",".bmp", ".tga", ".yaml",
        ".tif",  ".tiff",  ".gif", ".material"
    };
    return exts;
}

bool is_extension_allowed(const std::string& path)
{
    auto ext = std::filesystem::path(path).extension().string();
    std::transform(ext.begin(), ext.end(), ext.begin(), ::tolower);
    return allowed_extensions().count(ext) > 0;
}

bool has_traversal(const std::string& path)
{
    for (const auto& part : std::filesystem::path(path))
        if (part == ".." || part == ".")
            return true;
    return false;
}

bool is_path_safe(const std::string& base_dir, const std::string& full_path)
{
    try 
    {
        auto base   = std::filesystem::canonical(base_dir);
        auto target = std::filesystem::weakly_canonical(full_path);
        auto [end, _] = std::mismatch(base.begin(), base.end(), target.begin());
        return end == base.end();
    } catch (...) 
    {
        return false;
    }
}

// TODO: need better exception handling
bool validate_path(const std::string& filepath, const std::string& base_dir, const std::string& full_path)
{
    if (has_traversal(filepath)) 
    {
        ROS_WARN("Path traversal attempt blocked: %s", full_path.c_str());
        return false;
    }
    if (!is_extension_allowed(filepath)) 
    {
        ROS_WARN("Extension not allowed: %s", full_path.c_str());
        return false;
    }
    if (!is_path_safe(base_dir, full_path)) 
    {
        ROS_WARN("Path escapes package directory: %s", full_path.c_str());
        return false;
    }
    return true;
}

bool parse_package_url(const std::string& name, std::string& base_dir, std::string& filepath, std::string& full_path)
{
    if (name.compare(0, 10, "package://") != 0) 
    {
        ROS_WARN("Only \"package://\" addresses allowed.");
        return false;
    }
    std::string address = name.substr(10);
    std::string package = address.substr(0, address.find("/"));
    filepath  = address.substr(package.length());
    base_dir  = ros::package::getPath(package);
    full_path = base_dir + filepath;
    
    if (base_dir.empty()) 
    {
        ROS_WARN("Package not found: %s", package.c_str());
        return false;
    }
    return true;
}

// callbacks

bool get_file(
    file_server::GetBinaryFile::Request  &req,
    file_server::GetBinaryFile::Response &res)
{
    std::string base_dir, filepath, full_path;
    if (!parse_package_url(req.name, base_dir, filepath, full_path)) return true;
    if (!validate_path(filepath, base_dir, full_path)) return true;

    std::ifstream file(full_path, std::ios::binary);
    if (!file.is_open()) 
    {
        ROS_WARN("File not found: %s", full_path.c_str());
        return true;
    }

    res.value.assign(std::istreambuf_iterator<char>(file), std::istreambuf_iterator<char>());
    ROS_INFO("get_file: %s", req.name.c_str());
    return true;
}

bool save_file(
    file_server::SaveBinaryFile::Request  &req,
    file_server::SaveBinaryFile::Response &res)
{
    bool allow_save, allow_overwrite;
    ros::param::get("~allow_save",      allow_save);
    ros::param::get("~allow_overwrite", allow_overwrite);

    if (!allow_save) 
    {
        ROS_WARN("Save service is disabled. Enable with parameter \"allow_save\".");
        return true;
    }

    std::string base_dir, filepath, full_path;
    if (!parse_package_url(req.name, base_dir, filepath, full_path)) return true;
    if (!validate_path(filepath, base_dir, full_path)) return true;

    if (!allow_overwrite && std::filesystem::exists(full_path)) 
    {
        ROS_WARN("Overwrite blocked: %s", full_path.c_str());
        return true;
    }

    std::filesystem::create_directories(std::filesystem::path(full_path).parent_path());

    std::ofstream file(full_path, std::ios::binary);
    if (!file.is_open()) 
    {
        ROS_ERROR("Failed to open for write: %s", full_path.c_str());
        return true;
    }

    file.write(reinterpret_cast<const char*>(req.value.data()), req.value.size());
    res.name = req.name;
    ROS_INFO("save_file: %s", req.name.c_str());
    return true;
}

int main(int argc, char **argv)
{
    ros::init(argc, argv, "file_server");
    ros::NodeHandle n("~");

    bool allow_save      = false;
    bool allow_overwrite = false;
    n.param("allow_save",      allow_save,      false);
    n.param("allow_overwrite", allow_overwrite, false);

    ROS_INFO("Save: %s | Overwrite: %s", allow_save ? "enabled" : "disabled",
             (!allow_save || !allow_overwrite) ? "disabled" : "enabled");

    ros::ServiceServer get_service  = n.advertiseService("/file_server/get_file",  get_file);
    ros::ServiceServer save_service = n.advertiseService("/file_server/save_file", save_file);

    ROS_INFO("ROS1 File Server initialized.");
    ros::spin();
    return 0;
}