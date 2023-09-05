export type Alignment = "start" | "end";
export type Side = "top" | "right" | "bottom" | "left";
export type AlignedPlacement = `${Side}-${Alignment}`;
export type Placement = Side | AlignedPlacement;
export type Strategy = "absolute" | "fixed";
export type Axis = "x" | "y";
export type Length = "width" | "height";
export type SideObject = { [key in Side]: number };
export type Rect = { x: number; y: number; width: number; height: number };
export type Padding = number | Partial<SideObject>;
export type ClientRectObject = Rect & SideObject;

export interface ElementRects {
  reference: Rect;
  floating: Rect;
}

export type VirtualElement = {
  getBoundingClientRect(): ClientRectObject;
  contextElement?: any;
};
