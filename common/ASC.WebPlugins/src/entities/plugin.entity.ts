import { Entity, Column, PrimaryGeneratedColumn } from "typeorm";

@Entity()
export class Plugin {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  name: string;

  @Column()
  filename: string;

  @Column({ default: true })
  isActive: boolean;
}
