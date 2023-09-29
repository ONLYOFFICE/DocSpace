import styled from "styled-components";

const StyledHeader = styled.div`
  display: flex;
  align-items: center;

  div {
    color: #a3a9ae;

    // margin-left: 2px;
  }
`;

const Header = ({ t, name }) => {
  return (
    <StyledHeader>
      {t("Common:Settings")}&nbsp;
      <div>({name})</div>
    </StyledHeader>
  );
};

export default Header;
