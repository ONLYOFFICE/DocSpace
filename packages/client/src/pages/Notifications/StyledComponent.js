import styled from "styled-components";

const StyledSectionBodyContent = styled.div`
  .notification-container {
    display: grid;
    max-width: 660px;
    grid-template-columns: 1fr 120px;
    margin-bottom: 24px;

    .toggle-btn {
      padding-left: 46px;
    }
  }
  .badges-container {
    margin-bottom: 40px;
  }
`;

const StyledTextContent = styled.div`
  margin-bottom: 24px;
  height: 39px;
  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};
  max-width: 700px;
`;

const StyledSectionHeader = styled.div`
  display: flex;
  align-items: center;
  .arrow-button {
    margin-right: 16px;
  }
`;
export { StyledTextContent, StyledSectionBodyContent, StyledSectionHeader };
