import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";

import { PluginsModule } from "./plugins/plugins.module";

import { Plugin } from "./entities/plugin.entity";

@Module({
  imports: [
    TypeOrmModule.forRoot({
      type: "mysql",
      host: "localhost",
      port: 33060,
      username: "onlyoffice_user",
      password: "onlyoffice_pass",
      database: "docspace",
      entities: [Plugin],
      synchronize: true,
    }),
    PluginsModule,
  ],
})
export class AppModule {}
