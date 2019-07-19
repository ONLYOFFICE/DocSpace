import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';

export default function createStyledHeader(headlineType) {

   const fontSize = css`
      ${
         (headlineType === 'MenuHeader' && 27) ||
         (headlineType === 'ContentHeader' && 21)
      }
   `;

   const StyledHeadline = styled.h1`
      font-family: 'Open Sans',sans-serif,Arial;
      margin: 0;
      font-size: ${fontSize}px;
      font-weight: 700;
      color: ${props => props.isDisabled == true ? '#ECEEF1' : '#333333'};
      text-align: left;
      max-width: 1000px;
      ${props => (props.truncate === true && 'white-space: nowrap; overflow: hidden; text-overflow: ellipsis;' )}
      ${props => props.isInline == true && 'display: inline-block;'}
   `

   const Text = props => <StyledHeadline {...props} title={props.title}></StyledHeadline>

   Text.propTypes = {
      title: PropTypes.string,
      truncate: PropTypes.bool,
      isDisabled: PropTypes.bool,
      isInline: PropTypes.bool
   };

   Text.defaultProps = {
      title: '',
      truncate: false,
      isDisabled: false,
      isInline: false
   };

   return Text;
}