import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';


export default function createStyledHeadline() {

   const fontSize = css`
      ${props =>
         (props.tag === 'h1' && 23) ||
         (props.tag === 'h2' && 19) ||
         (props.tag === 'h3' && 15)
      }
   `;

   const styles = css`
      font-family: 'Open Sans',sans-serif,Arial;
      font-size: ${fontSize}px;
      font-weight: 600;
      color: ${props => props.isDisabled == true ? '#ECEEF1' : '#333333'};
      text-align: left;
      ${props => (props.truncate === true && 'white-space: nowrap; overflow: hidden; text-overflow: ellipsis;')}
      ${props => props.isInline == true && 'display: inline-block;'}
   `

   const StyledH1 = styled.h1`
      ${styles}
   `;

   const StyledH2 = styled.h2`
      ${styles}
   `;

   const StyledH3 = styled.h3`
      ${styles}
   `;

   const Text = props => {
      //console.log("Text.Headerline render");
      return (
         (props.tag === 'h1' && <StyledH1 {...props} title={props.title}></StyledH1>) ||
         (props.tag === 'h2' && <StyledH2 {...props} title={props.title}></StyledH2>) ||
         (props.tag === 'h3' && <StyledH3 {...props} title={props.title}></StyledH3>)
      );
   };

   Text.propTypes = {
      tag: PropTypes.oneOf(['h1', 'h2', 'h3']),
      title: PropTypes.string,
      truncate: PropTypes.bool,
      isDisabled: PropTypes.bool,
      isInline: PropTypes.bool
   };

   Text.defaultProps = {
      tag: 'h1',
      title: '',
      truncate: false,
      isDisabled: false,
      isInline: false
   };

   return Text;
}