import { Injectable } from "@nestjs/common";
import { InjectRepository } from "@nestjs/typeorm";
import { Repository } from "typeorm";
import * as path from "path";
import * as fs from "fs";

import * as config from "../../config";

const { path: pathToPlugins } = config.default.get("pluginsConf");

import { Plugin } from "src/entities/plugin.entity";

@Injectable()
export class PluginsService {
  constructor(
    @InjectRepository(Plugin)
    private pluginsRepository: Repository<Plugin>
  ) {}

  findAll(): Promise<Plugin[]> {
    return this.pluginsRepository.find();
  }

  findOne(id: number): Promise<Plugin> {
    return this.pluginsRepository.findOneBy({ id });
  }

  async add(dto: Plugin): Promise<Plugin> {
    return this.pluginsRepository.save(dto);
  }

  async remove(id: string): Promise<void> {
    await this.pluginsRepository.delete(id);
  }

  async activate(id: number): Promise<Plugin> {
    const plugin: Plugin = await this.pluginsRepository.findOneBy({ id });

    plugin.isActive = !plugin.isActive;

    await this.pluginsRepository.save(plugin);

    return plugin;
  }

  async uploadImg(id: number, filename: string) {
    const plugin = await this.pluginsRepository.findOneBy({ id });

    plugin.image = filename;

    await this.pluginsRepository.save(plugin);
  }

  async upload(id: number, filename: string) {
    const plugin = await this.pluginsRepository.findOneBy({ id });

    plugin.plugin = filename;

    await this.pluginsRepository.save(plugin);
  }

  async delete(id: number) {
    const plugin: Plugin = await this.pluginsRepository.findOneBy({ id });

    const fileName = plugin.plugin;

    const dir = path.join(__dirname, pathToPlugins, `${fileName}`);

    const imageName = plugin.image;

    const imageDir = path.join(__dirname, pathToPlugins, `${fileName}`);

    fs.unlink(dir, (err) => {
      err && console.log(err);
    });

    fs.unlink(imageDir, (err) => {
      err && console.log(err);
    });

    await this.pluginsRepository.delete(id);
  }
}
