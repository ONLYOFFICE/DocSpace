import type {
  Axis,
  ClientRectObject,
  ElementRects,
  Length,
  Placement,
  Rect,
  SideObject,
  Strategy,
  Padding,
  AlignedPlacement,
  Alignment,
} from "../utils/types";

export { SideObject, Padding, Alignment, AlignedPlacement, Placement };

type Promisable<T> = T | Promise<T>;

export type Derivable<T> = (state: MiddlewareState) => T;

export interface Platform {
  getElementRects: (args: {
    reference: ReferenceElement;
    floating: FloatingElement;
    strategy: Strategy;
  }) => Promisable<ElementRects>;
  getClippingRect: (args: {
    element: any;
    boundary: Boundary;
    rootBoundary: RootBoundary;
    strategy: Strategy;
  }) => Promisable<Rect>;
  getDimensions: (element: any) => Promisable<Dimensions>;

  // Optional
  convertOffsetParentRelativeRectToViewportRelativeRect?: (args: {
    rect: Rect;
    offsetParent: any;
    strategy: Strategy;
  }) => Promisable<Rect>;
  getOffsetParent?: (element: any) => Promisable<any>;
  isElement?: (value: any) => Promisable<boolean>;
  getDocumentElement?: (element: any) => Promisable<any>;
  getClientRects?: (element: any) => Promisable<Array<ClientRectObject>>;
  isRTL?: (element: any) => Promisable<boolean>;
  getScale?: (element: any) => Promisable<{ x: number; y: number }>;
}

export type Coords = { [key in Axis]: number };

export interface MiddlewareData {
  [key: string]: any;
  arrow?: Partial<Coords> & {
    centerOffset: number;
  };
  autoPlacement?: {
    index?: number;
    overflows: Array<{
      placement: Placement;
      overflows: Array<number>;
    }>;
  };
  flip?: {
    index?: number;
    overflows: Array<{
      placement: Placement;
      overflows: Array<number>;
    }>;
  };
  hide?: {
    referenceHidden?: boolean;
    escaped?: boolean;
    referenceHiddenOffsets?: SideObject;
    escapedOffsets?: SideObject;
  };
  offset?: Coords;
  shift?: Coords;
}

export interface ComputePositionConfig {
  platform: Platform;
  placement?: Placement;
  strategy?: Strategy;
  middleware?: Array<Middleware | null | undefined | false>;
}

export interface ComputePositionReturn extends Coords {
  placement: Placement;
  strategy: Strategy;
  middlewareData: MiddlewareData;
}

export type ComputePosition = (
  reference: unknown,
  floating: unknown,
  config: ComputePositionConfig
) => Promise<ComputePositionReturn>;

export interface MiddlewareReturn extends Partial<Coords> {
  data?: {
    [key: string]: any;
  };
  reset?:
    | true
    | {
        placement?: Placement;
        rects?: true | ElementRects;
      };
}

export type Middleware = {
  name: string;
  options?: any;
  fn: (state: MiddlewareState) => Promisable<MiddlewareReturn>;
};

export type Dimensions = { [key in Length]: number };

export type ReferenceElement = any;
export type FloatingElement = any;

export interface Elements {
  reference: ReferenceElement;
  floating: FloatingElement;
}

export interface MiddlewareState extends Coords {
  initialPlacement: Placement;
  placement: Placement;
  strategy: Strategy;
  middlewareData: MiddlewareData;
  elements: Elements;
  rects: ElementRects;
  platform: Platform;
}

export type MiddlewareArguments = MiddlewareState;

export type Boundary = any;
export type RootBoundary = "viewport" | "document" | Rect;
export type ElementContext = "reference" | "floating";

export type Options = Partial<{
  boundary: Boundary;
  rootBoundary: RootBoundary;
  elementContext: ElementContext;
  altBoundary: boolean;
  padding: Padding;
}>;
