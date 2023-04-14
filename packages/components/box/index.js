import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

const alignContentStyle = (alignContent) => `align-content: ${alignContent};`;
const alignItemsStyle = (alignItems) => `align-items: ${alignItems};`;
const alignSelfStyle = (alignSelf) => `align-self: ${alignSelf};`;
const backgroundStyle = (backgroundProp) => `background: ${backgroundProp};`;

const borderStyle = (borderProp) => {
  const styles = [];

  if (typeof borderProp === "string") {
    return `border: ${borderProp};`;
  }

  if (borderProp.style) styles.push(`border-style: ${borderProp.style};`);

  if (borderProp.width) styles.push(`border-width: ${borderProp.width};`);

  if (borderProp.color) styles.push(`border-color: ${borderProp.color};`);

  if (borderProp.radius) styles.push(`border-radius: ${borderProp.radius};`);

  return styles.join("\n");
};

const displayStyle = (displayProp) => `display: ${displayProp};`;
const flexBasisStyle = (flexBasis) => `flex-basis: ${flexBasis};`;
const flexDirectionStyle = (flexDirection) =>
  `flex-direction: ${flexDirection};`;
const flexStyle = (flexProp) => `flex: ${flexProp};`;
const flexWrapStyle = (flexWrap) => `flex-wrap: ${flexWrap};`;
const gridAreaStyle = (gridArea) => `grid-area: ${gridArea};`;
const heightStyle = (heightProp) => `height: ${heightProp};`;
const justifyContentStyle = (justifyContent) =>
  `justify-content: ${justifyContent};`;
const justifyItemsStyle = (justifyItems) => `justify-items: ${justifyItems};`;
const justifySelfStyle = (justifySelf) => `justify-self: ${justifySelf};`;
const marginStyle = (marginProp) => `margin: ${marginProp};`;
const overflowStyle = (overflowProp) => `overflow: ${overflowProp};`;
const paddingStyle = (paddingProp) => `padding: ${paddingProp};`;
const textAlignStyle = (textAlign) => `text-align: ${textAlign};`;
const widthStyle = (widthProp) => `width: ${widthProp};`;

const StyledBox = styled.div`
  ${(props) => props.alignContent && alignContentStyle(props.alignContent)}
  ${(props) => props.alignItems && alignItemsStyle(props.alignItems)}
  ${(props) => props.alignSelf && alignSelfStyle(props.alignSelf)}
  ${(props) => props.backgroundProp && backgroundStyle(props.backgroundProp)}
  ${(props) => props.borderProp && borderStyle(props.borderProp)}
  box-sizing: border-box;
  ${(props) => props.displayProp && displayStyle(props.displayProp)}
  ${(props) => props.flexBasis && flexBasisStyle(props.flexBasis)}
  ${(props) => props.flexDirection && flexDirectionStyle(props.flexDirection)}
  ${(props) => props.flexProp && flexStyle(props.flexProp)}
  ${(props) => props.flexWrap && flexWrapStyle(props.flexWrap)}
  ${(props) => props.gridArea && gridAreaStyle(props.gridArea)}
  ${(props) => props.heightProp && heightStyle(props.heightProp)}
  ${(props) =>
    props.justifyContent && justifyContentStyle(props.justifyContent)}
  ${(props) => props.justifyItems && justifyItemsStyle(props.justifyItems)}
  ${(props) => props.justifySelf && justifySelfStyle(props.justifySelf)}
  ${(props) => props.marginProp && marginStyle(props.marginProp)}
  outline: none;
  ${(props) => props.overflowProp && overflowStyle(props.overflowProp)}
  ${(props) => props.paddingProp && paddingStyle(props.paddingProp)}
  ${(props) => props.textAlign && textAlignStyle(props.textAlign)}
  ${(props) => props.widthProp && widthStyle(props.widthProp)}
`;

const Box = (props) => <StyledBox as={props.as} {...props} />;
Box.propTypes = {
  /** Sets the tag through which to render the component */
  as: PropTypes.string,
  /** Sets the distribution of space between and around content items
   * along a flexbox's cross-axis or a grid's block axis */
  alignContent: PropTypes.string,
  /** Sets the align-self value on all direct children as a group.
   * In Flexbox, it controls the alignment of items on the Cross Axis.
   * In Grid Layout, it controls the alignment of items on the Block Axis
   * within their grid area.  */
  alignItems: PropTypes.string,
  /** Overrides a grid or flex item's align-items value. In Grid,
   * it aligns the item inside the grid area. In Flexbox, it aligns
   * the item on the cross axis. */
  alignSelf: PropTypes.string,
  /** Sets all background style properties at once, such as color,
   * image, origin and size, or repeat method.  */
  backgroundProp: PropTypes.string,
  /** Sets the element's border. It sets the values of border-width,
   * border-style, and border-color. */
  borderProp: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  /** Sets whether the element is treated as a block or inline element and
   * the layout used for its children, such as flow layout, grid or flex. */
  displayProp: PropTypes.string,
  /** Sets the initial main size of the flex item. It sets the size of the content
   * box unless otherwise set with box-sizing. */
  flexBasis: PropTypes.string,
  /** Sets how flex items are placed in the flex container defining the main axis and
   * the direction (normal or reversed) */
  flexDirection: PropTypes.string,
  /** Sets how the flex item will grow or shrink to fit the space available in its
   * flex container. It is a shorthand for flex-grow, flex-shrink, and flex-basis. */
  flexProp: PropTypes.string,
  /** Sets whether flex items are forced onto one line or can wrap onto multiple lines.
   * If wrapping is allowed, it sets the direction that lines are stacked. */
  flexWrap: PropTypes.string,
  /** Is a shorthand property for grid-row-start, grid-column-start, grid-row-end and
   * grid-column-end, specifying the size of the grid item and location within the grid by
   * contributing a line, a span, or nothing (automatic) to its grid placement,
   * Thereby specifying the edges of its grid area. */
  gridArea: PropTypes.string,
  /** Defines the height of the border of the element area. */
  heightProp: PropTypes.string,
  /** Defines how the browser distributes space between and around content items along
   * the main-axis of a flex container, and the inline axis of a grid container */
  justifyContent: PropTypes.string,
  /** Defines the default justify-self for all items of the box, giving them all
   * a default way of justifying each box along the appropriate axis. */
  justifyItems: PropTypes.string,
  /** Sets the way the box is justified inside its alignment container along the appropriate axis. */
  justifySelf: PropTypes.string,
  /** Sets the margin area on all four sides of an element. It is a shorthand for margin-top,
   * margin-right, margin-bottom, and margin-left. */
  marginProp: PropTypes.string,
  /** Sets what to do when an element's content is too big to fit in its block formatting context. */
  overflowProp: PropTypes.string,
  /** Sets the padding area on all four sides of the element. It is a shorthand for padding-top,
   * padding-right, padding-bottom, and padding-left */
  paddingProp: PropTypes.string,
  /** Sets the horizontal alignment of a block element or table-cell box.
   * This means it works like vertical-align but in the horizontal direction  */
  textAlign: PropTypes.string,
  /** Defines the border width of the element area. */
  widthProp: PropTypes.string,
};

Box.defaultProps = {
  displayProp: "block",
};

export default Box;
