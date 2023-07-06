declare module "*.svg" {
  const SVG: React.VFC<React.SVGProps<SVGSVGElement>>;
  export default SVG;
}
declare module "*.svg?url" {
  const SVGUrl: string;
  export default SVGUrl;
}
