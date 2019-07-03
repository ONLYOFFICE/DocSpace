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
   color: ${props => props.isDisabled == true ? '#ECEEF1' : fontColor};
   ${props => props.backgroundColor == true && 'background-color: #F8F9F9;'}
   ${props => props.isInline == true && 'display: inline-block;'}
   text-align: left;
   max-width: 1000px;
   ${props => (props.truncate === true && 'white-space: nowrap; overflow: hidden; text-overflow: ellipsis;' )}

`

  const StyledP = styled.p`
   ${style}
  `;

  const StyledSpan = styled.span`
   ${style}
  `;

  const Text = props => 
      (props.tag === 'p' && <StyledP {...props} title={props.title}></StyledP>) ||
      (props.tag === 'span' && <StyledSpan {...props} title={props.title}></StyledSpan>) 

   Text.propTypes = {
     tag: PropTypes.oneOf(['p','span']),
     title: PropTypes.string,
     color: PropTypes.oneOf(['black', 'gray', 'lightGray']),
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
      backgroundColor: false,
      truncate: false,
      isDisabled: false,
      isBold: false,
      isInline: false,
      isItalic: false,
   };
  
  return Text;
}