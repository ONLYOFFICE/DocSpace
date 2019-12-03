import React from "react";
import { default as Header } from './text-header';

export { default as Headline } from "./text-headline";
export { default as Body } from "./text-body";
export const MenuHeader = (props) => <Header headlineType='MenuHeader' {...props} />;
export const ContentHeader = (props) => <Header headlineType='ContentHeader' {...props} />;