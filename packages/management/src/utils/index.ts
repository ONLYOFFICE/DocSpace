import { settingsTree } from "./settingsTree"

export const getItemByLink = (path: string) => {
    const resultPath = path.split("/")[2];
    const item = settingsTree.filter((item) => item.link === resultPath);
    return item[0];
}