import { DocumentProps } from "react-pdf";

interface PDFViewerProps {
  src?: string;
  handleChangeVersion: (arg: string) => void;
}

export default PDFViewerProps;

export type FirstArgument<T extends ((...args: any) => any) | undefined> =
  T extends (arg: infer R, ...args: any) => any ? R : never;

export type LoadSuccessType = DocumentProps["onLoadSuccess"];
export type PDFDocumentProxy = FirstArgument<LoadSuccessType>;
export type PDFPageProxy = Awaited<ReturnType<PDFDocumentProxy["getPage"]>>;
