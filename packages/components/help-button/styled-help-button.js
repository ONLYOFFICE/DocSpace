import styled from "styled-components";
import Base from "../themes/base";

const Content = styled.div`
  box-sizing: border-box;
  position: relative;
  width: ${(props) => props.theme.helpButton.width};
  background-color: ${(props) => props.theme.helpButton.backgroundColor};
  padding: ${(props) => props.theme.helpButton.padding};

  .header {
    max-width: ${(props) => props.theme.helpButton.maxWidth};
    margin: ${(props) => props.theme.helpButton.margin};
    line-height: ${(props) => props.theme.helpButton.lineHeight};
    font-weight: ${(props) => props.theme.helpButton.fontWeight};
  }
`;
Content.defaultProps = { theme: Base };

const HeaderContent = styled.div`
  display: flex;
  align-items: center;
  border-bottom: ${(props) => props.theme.helpButton.borderBottom};
`;
HeaderContent.defaultProps = { theme: Base };

const Body = styled.div`
  position: relative;
  padding: ${(props) => props.theme.helpButton.bodyPadding};
`;
Body.defaultProps = { theme: Base };

const StyledHelpContent = styled.div`
  cursor: pointer;
  line-height: 0;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  height: 15px;
  width: fit-content;
  p {
    max-width: fit-content;
    text-decoration: underline dashed;
  }
`;
StyledHelpContent.defaultProps = { theme: Base };

export { Content, HeaderContent, Body, StyledHelpContent };
