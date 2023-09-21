import { Body, Controller, Get, Post } from "@nestjs/common";
import { AppService } from "./app.service";
import { AppDto } from "./app.dto";

@Controller("sendtgreport")
export class AppController {
  constructor(private readonly appService: AppService) { }

  @Post()
  sendMessage(@Body() appDto: AppDto): Promise<string> {
    return this.appService.sendMessage(appDto);
  }
}
