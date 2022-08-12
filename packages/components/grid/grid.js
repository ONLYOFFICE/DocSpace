import React, { forwardRef } from "react";
import PropTypes from "prop-types";
import StyledGrid from "./styledGrid";

const Grid = forwardRef(({ tag, as, ...rest }, ref) => {
  //console.log("Grid render", rest)
  return <StyledGrid as={!as && tag ? tag : as} ref={ref} {...rest} />;
});

Grid.propTypes = {
  as: PropTypes.string,
  /** sets the distribution of space between and around content items along a flexbox's cross-axis or a grid's block axis */
  alignContent: PropTypes.string,
  /** sets the align-self value on all direct children as a group. In Flexbox, it controls the alignment of items on the Cross Axis.
   * In Grid Layout, it controls the alignment of items on the Block Axis within their grid area. */
  alignItems: PropTypes.string,
  /** overrides a grid or flex item's align-items value. In Grid, it aligns the item inside the grid area.
   * In Flexbox, it aligns the item on the cross axis. */
  alignSelf: PropTypes.string,
  /** 	specifies named grid areas. Takes value as array of string arrays that specify named grid areas.
   * Or objects array, that contains names and coordinates of areas. */
  areasProp: PropTypes.array,
  /** defines the sizing of the grid columns. Specifying a single string will repeat several columns of this size.
   * Specifying an object allows you to specify the number of repetitions and the size of the column.
   * Or you can specify an array with column sizes. The column size can be specified as an array of minimum and maximum widths. */
  columnsProp: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.array,
    PropTypes.object,
  ]),
  /** is a shorthand property for grid-row-start, grid-column-start, grid-row-end and grid-column-end, specifying a grid item’s size and
   * location within the grid by contributing a line, a span, or nothing (automatic) to its grid placement,
   *  thereby specifying the edges of its grid area. */
  gridArea: PropTypes.string,
  /** sets the size of the gap (gutter) between an element's columns. */
  gridColumnGap: PropTypes.string,
  /** sets the gaps (gutters) between rows and columns. It is a shorthand for row-gap and column-gap. */
  gridGap: PropTypes.string,
  /** sets the size of the gap (gutter) between an element's grid rows. */
  gridRowGap: PropTypes.string,
  /** defines the height of the border of the element area. */
  heightProp: PropTypes.string,
  /** defines how the browser distributes space between and around content items along the main-axis of a flex container,
   * and the inline axis of a grid container. */
  justifyContent: PropTypes.string,
  /** defines the default justify-self for all items of the box, giving them all a default way of justifying each box along the appropriate axis. */
  justifyItems: PropTypes.string,
  /** sets the way a box is justified inside its alignment container along the appropriate axis. */
  justifySelf: PropTypes.string,
  /** sets the margin area on all four sides of an element. It is a shorthand for margin-top, margin-right, margin-bottom, and margin-left */
  marginProp: PropTypes.string,
  /** sets the padding area on all four sides of an element. It is a shorthand for padding-top, padding-right, padding-bottom, and padding-left */
  paddingProp: PropTypes.string,
  /** defines the sizing of the grid rows. Specifying a single string will repeat several rows of this size. Or you can specify an array with rows sizes.
   * The row size can be specified as an array of minimum and maximum heights. */
  rowsProp: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  tag: PropTypes.string,
  /** defines the width of the border of the element area. */
  widthProp: PropTypes.string,
};

Grid.defaultProps = {
  heightProp: "100%",
  widthProp: "100%",
};

Grid.displayName = "Grid";

export default Grid;
