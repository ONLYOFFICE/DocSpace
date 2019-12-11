import styled from 'styled-components';
import PropTypes from 'prop-types';

const alignContentStyle = alignContent => `align-content: ${alignContent};`;
const alignItemsStyle = alignItems => `align-items: ${alignItems};`;
const alignSelfStyle = alignSelf => `align-self: ${alignSelf};`;
const backgroundStyle = backgroundProp => `background: ${backgroundProp};`;

const borderStyle = borderProp => {
  const styles = [];

  if (typeof borderProp === 'string') {
    return `border: ${borderProp};`;
  }

  if (borderProp.style)
    styles.push(`border-style: ${borderProp.style};`);

  if (borderProp.width)
    styles.push(`border-width: ${borderProp.width};`);

  if (borderProp.color)
    styles.push(`border-color: ${borderProp.color};`); 

  if (borderProp.radius)
    styles.push(`border-radius: ${borderProp.radius};`);

  return styles.join('\n');
} 

const displayStyle = displayProp => `display: ${displayProp};`;
const flexBasisStyle = flexBasis => `flex-basis: ${flexBasis};`;
const flexDirectionStyle = flexDirection => `flex-direction: ${flexDirection};`;
const flexStyle = flexProp => `flex: ${flexProp};`;
const flexWrapStyle = flexWrap => `flex-wrap: ${flexWrap};`;
const gridAreaStyle = gridArea => `grid-area: ${gridArea};`;
const heightStyle = heightProp => `height: ${heightProp};`;
const justifyContentStyle = justifyContent => `justify-content: ${justifyContent};`;
const justifyItemsStyle = justifyItems => `justify-items: ${justifyItems};`;
const justifySelfStyle = justifySelf => `justify-self: ${justifySelf};`;
const marginStyle = marginProp => `margin: ${marginProp};`;
const overflowStyle = overflowProp => `overflow: ${overflowProp};`;
const paddingStyle = paddingProp => `padding: ${paddingProp};`;
const textAlignStyle = textAlign => `text-align: ${textAlign};`;
const widthStyle = widthProp => `width: ${widthProp};`;

const Box  = styled.div`
  ${props => props.alignContent && alignContentStyle(props.alignContent)}
  ${props => props.alignItems && alignItemsStyle(props.alignItems)}
  ${props => props.alignSelf && alignSelfStyle(props.alignSelf)}
  ${props => props.backgroundProp && backgroundStyle(props.backgroundProp)}
  ${props => props.borderProp && borderStyle(props.borderProp)}
  box-sizing: border-box;
  ${props => props.displayProp && displayStyle(props.displayProp)}
  ${props => props.flexBasis && flexBasisStyle(props.flexBasis)}
  ${props => props.flexDirection && flexDirectionStyle(props.flexDirection)}
  ${props => props.flexProp && flexStyle(props.flexProp)}
  ${props => props.flexWrap && flexWrapStyle(props.flexWrap)}
  ${props => props.gridArea && gridAreaStyle(props.gridArea)}
  ${props => props.heightProp && heightStyle(props.heightProp)}
  ${props => props.justifyContent && justifyContentStyle(props.justifyContent)}
  ${props => props.justifyItems && justifyItemsStyle(props.justifyItems)}
  ${props => props.justifySelf && justifySelfStyle(props.justifySelf)}
  ${props => props.marginProp && marginStyle(props.marginProp)}
  outline: none;
  ${props => props.overflowProp && overflowStyle(props.overflowProp)}
  ${props => props.paddingProp && paddingStyle(props.paddingProp)}
  ${props => props.textAlign && textAlignStyle(props.textAlign)}
  ${props => props.widthProp && widthStyle(props.widthProp)}
`;

Box.propTypes = {
  alignContent: PropTypes.string,
  alignItems: PropTypes.string,
  alignSelf: PropTypes.string,
  backgroundProp: PropTypes.string,
  borderProp: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  displayProp: PropTypes.string,
  flexBasis: PropTypes.string,
  flexDirection: PropTypes.string,
  flexProp: PropTypes.string,
  flexWrap: PropTypes.string,
  gridArea: PropTypes.string,
  heightProp: PropTypes.string,
  justifyContent: PropTypes.string,
  justifyItems: PropTypes.string,
  justifySelf: PropTypes.string,
  marginProp: PropTypes.string,
  overflowProp: PropTypes.string,
  paddingProp: PropTypes.string,
  textAlign: PropTypes.string,
  widthProp: PropTypes.string
}

Box.defaultProps = {
  displayProp: 'block'
};

export default Box;
