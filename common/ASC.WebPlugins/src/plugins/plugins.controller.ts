import {
  Controller,
  Param,
  Body,
  Get,
  Post,
  UploadedFiles,
  UseInterceptors,
  Put,
  Delete,
  UseGuards,
} from "@nestjs/common";

import { AnyFilesInterceptor } from "@nestjs/platform-express";

import { storage } from "src/utils";

import { Plugin } from "src/entities/plugin.entity";

import {
  PluginGuard,
  PluginUploadGuard,
  PluginDeleteGuard,
} from "src/guards/plugin.guard";

import { PluginsService } from "./plugins.service";

@Controller("/api/2.0/plugins")
@UseGuards(PluginGuard)
export class PluginsController {
  constructor(private pluginsService: PluginsService) {}

  @Get()
  async findAll(): Promise<Plugin[]> {
    const plugins: Plugin[] = await this.pluginsService.findAll();
    return plugins;
  }

  @Put("activate/:id")
  async activate(@Param("id") id: number): Promise<Plugin> {
    return this.pluginsService.activate(id);
  }

  @Post("upload")
  @UseGuards(PluginUploadGuard)
  @UseInterceptors(
    AnyFilesInterceptor({
      storage: storage,
    })
  )
  upload(@UploadedFiles() files: Express.Multer.File[]) {
    return this.pluginsService.upload(files[0].originalname, files[0].filename);
  }

  @Delete("delete/:id")
  @UseGuards(PluginDeleteGuard)
  delete(@Param("id") id: number) {
    this.pluginsService.delete(id);
  }
}

export default PluginsController;
