import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';


export default function createStyledBodyText() {

const fontColor = css`
   ${props =>
      (props.color === 'black' && '#333333') ||
      (props.color === 'gray' && '#657077') ||
      (props.color === 'lightGray' && '#A3A9AE') 
   }
`

const style = css`
   font-family: 'Open Sans',sans-serif,Arial;
   font-size: 13px;
   font-weight: ${props => props.isBold == true ? 700 : 500};
   ${props => props.isItalic == true && 'font-style: italic'};
   color: ${props => props.isDisabled == true ? props.disableColor : fontColor};
   ${props => props.backgroundColor == true && 'background-color: #F8F9F9;'}
   ${props => props.isInline == true && 'display: inline-block;'}
   text-align: left;
   ${props => (props.truncate === true && 'white-space: nowrap; overflow: hidden; text-overflow: ellipsis;' )}

`

const TextBody = styled.p`
   ${style}
`;

  const Text = props => {
     //console.log("Text render");
     return (<TextBody as={props.tag} {...props} title={props.title}></TextBody>);
  };

   Text.propTypes = {
      tag: PropTypes.string,
      title: PropTypes.string,
      color: PropTypes.oneOf(['black', 'gray', 'lightGray']),
      disableColor: PropTypes.string,
      backgroundColor: PropTypes.bool,
      truncate: PropTypes.bool,
      isDisabled: PropTypes.bool,
      isBold: PropTypes.bool,
      isInline: PropTypes.bool,
      isItalic: PropTypes.bool
  };

   Text.defaultProps = {
      tag: 'p',
      title: '',
      color: 'black',
      disableColor: '#ECEEF1',
      backgroundColor: false,
      truncate: false,
      isDisabled: false,
      isBold: false,
      isInline: false,
      isItalic: false,
   };
  
  return Text;
}