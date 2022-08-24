import { Injectable, CanActivate, ExecutionContext } from "@nestjs/common";
import { Observable } from "rxjs";

import * as config from "../../../../config/appsettings.json";

@Injectable()
export class PluginGuard implements CanActivate {
  canActivate(
    context: ExecutionContext
  ): boolean | Promise<boolean> | Observable<boolean> {
    console.log(config);

    return config?.plugins?.enabled === "true";
  }
}

@Injectable()
export class PluginUploadGuard implements CanActivate {
  canActivate(
    context: ExecutionContext
  ): boolean | Promise<boolean> | Observable<boolean> {
    return (
      config?.plugins?.enabled === "true" &&
      config?.plugins?.allow.includes("upload")
    );
  }
}

@Injectable()
export class PluginDeleteGuard implements CanActivate {
  canActivate(
    context: ExecutionContext
  ): boolean | Promise<boolean> | Observable<boolean> {
    return (
      config?.plugins?.enabled === "true" &&
      config?.plugins?.allow.includes("delete")
    );
  }
}
