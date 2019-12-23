import styled, { css } from 'styled-components';
import { Heading } from 'asc-web-components';

const fontSize = css`
      ${props =>
      (props.type === 'menu' && 27) ||
      (props.type === 'content' && 21)
   }
   `;

const StyledHeading = styled(Heading)`
   margin: 0;
   line-height: 56px;
   font-size: ${fontSize}px;
   font-weight: 700;
`;

export default StyledHeading;