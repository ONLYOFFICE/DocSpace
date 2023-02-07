import { Injectable, CanActivate, ExecutionContext } from "@nestjs/common";
import { Observable } from "rxjs";

import * as config from "../../config";

const { enabled, allow } = config.default.get("plugins");

@Injectable()
export class PluginGuard implements CanActivate {
  canActivate(
    context: ExecutionContext
  ): boolean | Promise<boolean> | Observable<boolean> {
    return enabled === "true";
  }
}

@Injectable()
export class PluginUploadGuard implements CanActivate {
  canActivate(
    context: ExecutionContext
  ): boolean | Promise<boolean> | Observable<boolean> {
    return enabled === "true" && allow.includes("upload");
  }
}

@Injectable()
export class PluginDeleteGuard implements CanActivate {
  canActivate(
    context: ExecutionContext
  ): boolean | Promise<boolean> | Observable<boolean> {
    return enabled === "true" && allow.includes("delete");
  }
}
