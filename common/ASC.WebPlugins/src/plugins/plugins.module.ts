import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";

import { PluginsController } from "./plugins.controller";
import { PluginsService } from "./plugins.service";

import { Plugin } from "src/entities/plugin.entity";

@Module({
  imports: [TypeOrmModule.forFeature([Plugin])],
  controllers: [PluginsController],
  providers: [PluginsService],
})
export class PluginsModule {}
