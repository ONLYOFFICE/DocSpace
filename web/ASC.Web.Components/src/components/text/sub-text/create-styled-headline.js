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

   const StyledHeadline = styled.h1`
      ${styles}
   `;

   const Text = props => {
      return (
         <StyledHeadline as={props.tag} {...props} title={props.title}></StyledHeadline>
      );
   };

   Text.propTypes = {
      tag: PropTypes.string,
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