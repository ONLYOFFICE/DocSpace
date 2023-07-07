type TLogoPath = {
    light: string;
    dark?: string;
}

type TLogoSize = {
    width: number;
    height: number;
    isEmpty: boolean;
}

export type TLogoUrl = {
    name: string;
    path: TLogoPath;
    size: TLogoSize;
}
