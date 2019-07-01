import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';

export default function createStyledMainHeadline() {

   const fontSize = css`
      ${props =>
         (props.headlineName === 'moduleName' && 27) ||
         (props.headlineName === 'mainTitle' && 21)
      }
   `;

   const StyledHeadline = styled.h1`
      font-family: 'Open Sans',sans-serif,Arial;
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
      headlineName: PropTypes.oneOf(['moduleName', 'mainTitle']),
      title: PropTypes.string,
      truncate: PropTypes.bool,
      isDisabled: PropTypes.bool,
      isInline: PropTypes.bool
   };

   Text.defaultProps = {
      headlineName: 'moduleName',
      title: '',
      truncate: false,
      isDisabled: false,
      isInline: false
   };

   return Text;
}