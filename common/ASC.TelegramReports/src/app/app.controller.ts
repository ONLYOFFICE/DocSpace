import { Body, Controller, Get, Post } from "@nestjs/common";
import { AppService } from "./app.service";
import { AppDto } from "./app.dto";

@Controller("/api/2.0/sendtgreport")
export class AppController {
  constructor(private readonly appService: AppService) { }

  @Post()
  async sendMessage(@Body() appDto: AppDto): Promise<string> {
    return await this.appService.sendMessage(appDto);
  }
}
