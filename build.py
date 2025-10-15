
import os
import sys
import subprocess
import platform
import shutil

root_dir = os.path.dirname(os.path.realpath(__file__))

print(f"root directory - {root_dir}")

if os.path.exists(f"{root_dir}/binary"):
    shutil.rmtree(f"{root_dir}/binary")
if os.path.exists(f"{root_dir}/release"):
    shutil.rmtree(f"{root_dir}/release")

target_mode = sys.argv[1].lower()
target_os = sys.argv[2]
target_arch = sys.argv[3]
target_aot = sys.argv[4]

subprocess.run(['cargo', 'build', f'--release','-Z','unstable-options','--artifact-dir',f'{root_dir}/binary'],
                cwd=f'{root_dir}/native')

if target_os == 'auto':
    if sys.platform.startswith('win'):
        target_os = 'win'
    elif sys.platform.startswith('linux'):
        target_os = 'linux'
    else:
        target_os = 'osx'

if target_arch == 'auto':
    if platform.machine().lower().startswith('amd64'):
        target_arch = 'x64'
    else:
        target_arch = 'aarch64'

subprocess.run(['dotnet','publish', '-r', f'{target_os}-{target_arch}','-c',target_mode.title(), 
                f'{root_dir}/zmake/ZMake.csproj', f'--property:ZMakeAOT={target_aot}',
                '--output', f'{root_dir}/release'])
