import styled, { css } from "styled-components";

const alignContentStyle = (alignContent) => `align-content: ${alignContent};`;
const alignItemsStyle = (alignItems) => `align-items: ${alignItems};`;
const alignSelfStyle = (alignSelf) => `align-self: ${alignSelf};`;

const areasStyle = (props) => {
  if (
    Array.isArray(props.areasProp) &&
    props.areasProp.every((area) => Array.isArray(area))
  ) {
    return `grid-template-areas: ${props.areasProp
      .map((area) => `"${area.join(" ")}"`)
      .join(" ")};`;
  }
  const cells = props.rowsProp.map(() => props.columnsProp.map(() => "."));
  props.areasProp.forEach((area) => {
    for (let row = area.start[1]; row <= area.end[1]; row += 1) {
      for (let column = area.start[0]; column <= area.end[0]; column += 1) {
        cells[row][column] = area.name;
      }
    }
  });
  return `grid-template-areas: ${cells
    .map((r) => `"${r.join(" ")}"`)
    .join(" ")};`;
};

const getSizeValue = (value) =>
  Array.isArray(value) ? `minmax(${value[0]}, ${value[1]})` : value;

const columnsStyle = (props) => {
  if (Array.isArray(props.columnsProp)) {
    return css`
      grid-template-columns: ${props.columnsProp.map(getSizeValue).join(" ")};
    `;
  }
  if (typeof props.columnsProp === "object") {
    return css`
      grid-template-columns: repeat(
        ${props.columnsProp.count},
        ${getSizeValue(props.columnsProp.size)}
      );
    `;
  }
  return css`
    grid-template-columns: repeat(
      auto-fill,
      ${getSizeValue(props.columnsProp)}
    );
  `;
};

const gridAreaStyle = (gridArea) => `grid-area: ${gridArea};`;
const gridColumnGapStyle = (gridColumnGap) =>
  `grid-column-gap: ${gridColumnGap};`;
const gridGapStyle = (gridGap) => `grid-gap: ${gridGap};`;
const gridRowGapStyle = (gridRowGap) => `grid-row-gap: ${gridRowGap};`;
const heightStyle = (heightProp) => `height: ${heightProp};`;
const justifyContentStyle = (justifyContent) =>
  `justify-content: ${justifyContent};`;
const justifyItemsStyle = (justifyItems) => `justify-items: ${justifyItems};`;
const justifySelfStyle = (justifySelf) => `justify-self: ${justifySelf};`;
const marginStyle = (marginProp) => `margin: ${marginProp};`;
const paddingStyle = (paddingProp) => `padding: ${paddingProp};`;

const rowsStyle = (props) => {
  if (Array.isArray(props.rowsProp)) {
    return css`
      grid-template-rows: ${props.rowsProp.map(getSizeValue).join(" ")};
    `;
  }
  return css`
    grid-auto-rows: ${props.rowsProp};
  `;
};

const widthStyle = (widthProp) => `width: ${widthProp};`;

const StyledGrid = styled.div`
  ${(props) => props.alignContent && alignContentStyle(props.alignContent)}
  ${(props) => props.alignItems && alignItemsStyle(props.alignItems)}
  ${(props) => props.alignSelf && alignSelfStyle(props.alignSelf)}
  ${(props) => props.areasProp && areasStyle(props)}
  ${(props) => props.columnsProp && columnsStyle(props)}
  display: grid;
  ${(props) => props.gridArea && gridAreaStyle(props.gridArea)}
  ${(props) => props.gridColumnGap && gridColumnGapStyle(props.gridColumnGap)}
  ${(props) => props.gridGap && gridGapStyle(props.gridGap)}
  ${(props) => props.gridRowGap && gridRowGapStyle(props)}
  ${(props) => props.heightProp && heightStyle(props.heightProp)}
  ${(props) =>
    props.justifyContent && justifyContentStyle(props.justifyContent)}
  ${(props) => props.justifyItems && justifyItemsStyle(props.justifyItems)}
  ${(props) => props.justifySelf && justifySelfStyle(props.justifySelf)}
  ${(props) => props.marginProp && marginStyle(props.marginProp)}
  ${(props) => props.paddingProp && paddingStyle(props.paddingProp)}
  ${(props) => props.rowsProp && rowsStyle(props)}
  ${(props) => props.widthProp && widthStyle(props.widthProp)}
`;

export default StyledGrid;
