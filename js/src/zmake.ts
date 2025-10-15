
import { semver,chalk } from "./lib"

export { chalk };
export { semver };

//import * as internal from "zmake.internal";

export const version:semver.SemVer = new semver.SemVer("1");

export class ZMakeError extends Error {
    constructor(message:string, options:ErrorOptions | undefined = undefined) {
        super(message, options);
    }
}

export function requireVersion(versionConstraint:string){
    if(!semver.satisfies(version,
        versionConstraint)){
            throw new ZMakeError(
                `current zmake version ${version} not match constraint ${versionConstraint}"`);
        }
}

export const groupIdRegex = /^[a-z]+(\.[a-z])+$/;

export function isValidGroupId(groupId:string):boolean{
    return groupId.match(groupIdRegex) != null;
}

export const artifactIdRegex = /^[a-z]+$/

export function isValidArtifactId(artifactId:string):boolean{
    return artifactId.match(artifactIdRegex) != null;
}

export class Artifact{
    public groupId:string;
    public artifactId:string;
    public version:semver.SemVer;

    constructor(groupId:string,artifactId:string,version:string){
        this.groupId = groupId;
        this.artifactId = artifactId;
        this.version = new semver.SemVer(version);
    }
}

export class Name{
    public artifact:Artifact;
    public names:string[];

    constructor(artifact:Artifact,names:string[]){
        this.artifact = artifact;
        this.names = names;
    }
}
