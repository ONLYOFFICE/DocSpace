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
} from "@nestjs/common";

import { AnyFilesInterceptor } from "@nestjs/platform-express";

import { storage } from "src/utils";

import { Plugin } from "src/entities/plugin.entity";

import { PluginsService } from "./plugins.service";

@Controller("/api/2.0/plugins")
export class PluginsController {
  constructor(private pluginsService: PluginsService) {}

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

  @Post("add")
  async add(@Body() dto: Plugin): Promise<{ response: Plugin }> {
    const plugin: Plugin = await this.pluginsService.add(dto);

    return { response: plugin };
  }

  @Put("upload/image/:id")
  @UseInterceptors(
    AnyFilesInterceptor({
      storage: storage,
    })
  )
  async uploadImage(
    @UploadedFiles() file: Express.Multer.File,
    @Param("id") id: number
  ) {
    try {
      if (file[0]) {
        await this.pluginsService.uploadImg(id, file[0].filename);
      }
    } catch (e) {
      console.log(e);
    }
  }

  @Put("upload/plugin/:id")
  @UseInterceptors(
    AnyFilesInterceptor({
      storage: storage,
    })
  )
  async upload(
    @UploadedFiles() file: Express.Multer.File,
    @Param("id") id: number
  ) {
    try {
      if (file[0]) {
        await this.pluginsService.upload(id, file[0].filename);
      }
    } catch (e) {
      console.log(e);
    }
  }

  @Delete("delete/:id")
  async delete(@Param("id") id: number) {
    await this.pluginsService.delete(id);
  }
}

export default PluginsController;
