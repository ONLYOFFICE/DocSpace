import styled from 'styled-components';
import PropTypes from 'prop-types';

const displayStyle = displayProp => `display: ${displayProp};`;
const gridAreaStyle = gridArea => `grid-area: ${gridArea};`;
const flexBasisStyle = flexBasis => `flex-basis: ${flexBasis};`;
const flexWrapStyle = flexWrap => `flex-wrap: ${flexWrap};`;
const flexDirectionStyle = flexDirection => `flex-direction: ${flexDirection};`;
const alignItemsStyle = alignItems => `align-items: ${alignItems};`;
const alignContentStyle = alignContent => `align-content: ${alignContent};`;
const justifyContentStyle = justifyContent => `justify-content: ${justifyContent};`;
const overflowStyle = overflowProp => `overflow: ${overflowProp};`;
const heightStyle = heightProp => `height: ${heightProp};`;
const widthStyle = widthProp => `width: ${widthProp};`;

const Box  = styled.div`
  box-sizing: border-box;
  outline: none;

  ${props => props.displayProp && displayStyle(props.displayProp)}
  ${props => props.gridArea && gridAreaStyle(props.gridArea)}
  ${props => props.flexBasis && flexBasisStyle(props.flexBasis)}
  ${props => props.flexWrap && flexWrapStyle(props.flexWrap)}
  ${props => props.flexDirection && flexDirectionStyle(props.flexDirection)}
  ${props => props.alignItems && alignItemsStyle(props.alignItems)}
  ${props => props.alignContent && alignContentStyle(props.alignContent)}
  ${props => props.justifyContent && justifyContentStyle(props.justifyContent)}
  ${props => props.overflowProp && overflowStyle(props.overflowProp)}
  ${props => props.heightProp && heightStyle(props.heightProp)}
  ${props => props.widthProp && widthStyle(props.widthProp)}
`;

Box.propTypes = {
  displayProp: PropTypes.string,
  gridArea: PropTypes.string,
  flexBasis: PropTypes.string,
  flexWrap: PropTypes.string,
  flexDirection: PropTypes.string,
  alignItems: PropTypes.string,
  alignContent: PropTypes.string,
  justifyContent: PropTypes.string,
  overflowProp: PropTypes.string,
  heightProp: PropTypes.string,
  widthProp: PropTypes.string,
}

Box.defaultProps = {
  displayProp: 'block'
};

export default Box;
