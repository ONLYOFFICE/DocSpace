import styled from "styled-components";

const StyledSeveralItemsContainer = styled.div`
  margin: 80px auto 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 32px;
  padding-top: ${(props) => (props.isAccounts ? "80px" : "0")};

  img {
    width: 75px;
    height: 75px;
  }
`;

export { StyledSeveralItemsContainer };
