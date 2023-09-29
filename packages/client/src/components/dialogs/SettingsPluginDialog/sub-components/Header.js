import styled from "styled-components";

import { Base } from "@docspace/components/themes";

const StyledHeader = styled.div`
  display: flex;
  align-items: center;

  div {
    color: ${(props) => props.theme.plugins.pluginName};
  }
`;

StyledHeader.defaultProps = { theme: Base };

const Header = ({ t, name }) => {
  return (
    <StyledHeader>
      {t("Common:Settings")}&nbsp;
      <div>({name})</div>
    </StyledHeader>
  );
};

export default Header;
