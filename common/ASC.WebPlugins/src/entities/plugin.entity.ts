import { Entity, Column, PrimaryGeneratedColumn } from "typeorm";

@Entity()
export class Plugin {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  name: string;

  @Column()
  version: string;

  @Column()
  author: string;

  @Column()
  uploader: string;

  @Column()
  description: string;

  @Column()
  image: string;

  @Column()
  plugin: string;

  @Column()
  apiScope: boolean;

  @Column()
  settingsScope: boolean;

  @Column()
  contextMenuScope: boolean;

  @Column()
  mainButtonScope: boolean;

  @Column()
  profileMenuScope: boolean;

  @Column({ default: true })
  isActive: boolean;
}
