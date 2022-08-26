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

  async remove(id: string): Promise<void> {
    await this.pluginsRepository.delete(id);
  }

  async activate(id: number): Promise<Plugin> {
    const plugin: Plugin = await this.pluginsRepository.findOneBy({ id });

    plugin.isActive = !plugin.isActive;

    await this.pluginsRepository.save(plugin);

    return plugin;
  }

  async upload(originalname: string, filename: string): Promise<Plugin> {
    const plugin: Plugin = new Plugin();

    const name = originalname.split(".")[0];

    plugin.name = name;
    plugin.filename = filename;

    plugin.isActive = true;

    await this.pluginsRepository.save(plugin);

    return plugin;
  }

  async delete(id: number) {
    const plugin: Plugin = await this.pluginsRepository.findOneBy({ id });

    const fileName = plugin.filename;

    const dir = path.join(__dirname, pathToPlugins, `${fileName}`);

    fs.unlink(dir, (err) => {
      err && console.log(err);
    });

    await this.pluginsRepository.delete(id);
  }
}
