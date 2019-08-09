import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';


export default function createStyledBodyText() {

   const style = css`
   font-family: 'Open Sans',sans-serif,Arial;
   font-size: ${props => props.fontSize}px;
   font-weight: ${props => props.fontWeight
         ? props.fontWeight
         : props.isBold == true ? 700 : 500};
   ${props => props.isItalic == true && 'font-style: italic'};
   color: ${props => props.color};
   ${props => props.backgroundColor && 'background-color: ' + props.backgroundColor + ";"}
   ${props => props.isInline == true && 'display: inline-block;'}
   text-align: left;
   ${props => (props.truncate === true && 'white-space: nowrap; overflow: hidden; text-overflow: ellipsis;')}
   margin: 0;

`

   const TextBody = styled.p`
   ${style}
`;

   const Text = props => {
      //console.log("Text render");
      return (<TextBody {...props} title={props.title}></TextBody>);
   };

   Text.propTypes = {
      title: PropTypes.string,
      color: PropTypes.string,
      fontSize: PropTypes.number,
      fontWeight: PropTypes.number,
      backgroundColor: PropTypes.string,
      truncate: PropTypes.bool,
      isBold: PropTypes.bool,
      isInline: PropTypes.bool,
      isItalic: PropTypes.bool
   };

   Text.defaultProps = {
      title: '',
      color: '#333333',
      fontSize: 13,
      truncate: false,
      isBold: false,
      isInline: false,
      isItalic: false,
   };

   return Text;
}