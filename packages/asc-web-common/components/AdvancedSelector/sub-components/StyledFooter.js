import styled, { css } from 'styled-components';
import Base from '../../../../asc-web-components/themes/base';

const StyledFooter = styled.div`
  box-sizing: border-box;
  border-top: ${(props) => props.theme.advancedSelector.footerBorder};
  padding: 16px;
  height: 69px;

  ${(props) =>
    props.withEmbeddedComponent &&
    css`
      display: flex;
    `}

  ${(props) =>
    !props.isVisible &&
    css`
      display: none;
    `}
`;

StyledFooter.defaultProps = { theme: Base };

export default StyledFooter;
