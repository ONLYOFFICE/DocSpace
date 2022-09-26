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
import fileFilter from "src/utils/file-filter";

@Controller("/api/2.0/plugins")
@UseGuards(PluginGuard)
export class PluginsController {
  constructor(private pluginsService: PluginsService) { }

  @Get()
  async findAll(): Promise<{ response: Plugin[] }> {
    const plugins: Plugin[] = await this.pluginsService.findAll();
    return { response: plugins };
  }

  @Put("activate/:id")
  async activate(@Param("id") id: number): Promise<{ response: Plugin }> {
    const plugin: Plugin = await this.pluginsService.activate(id);
    return { response: plugin };
  }

  @Post("upload")
  @UseGuards(PluginUploadGuard)
  @UseInterceptors(
    AnyFilesInterceptor({
      storage: storage,
      fileFilter: fileFilter,
    })
  )
  async upload(
    @UploadedFiles() files: Express.Multer.File[]
  ): Promise<{ response: Plugin | { error: string } }> {
    try {
      if (files[0]) {
        const plugin = await this.pluginsService.upload(
          files[0].originalname,
          files[0].filename
        );

        return { response: plugin };
      } else {
        return {
          response: { error: "Invalid file format or file already exists" },
        };
      }
    } catch (e) {
      console.log(e);
      return {
        response: { error: "Invalid file format or file already exists" },
      };
    }
  }

  @Delete("delete/:id")
  @UseGuards(PluginDeleteGuard)
  async delete(@Param("id") id: number) {
    await this.pluginsService.delete(id);
  }
}

export default PluginsController;
