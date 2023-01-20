import React from "react";
import UsefulTipsContainer from "./sub-components/UsefulTipsContainer";
import styled from "styled-components";
import RoomsActionsContainer from "./sub-components/RoomsActionsContainer";
import DailyFeedContainer from "./sub-components/DailyFeedContainer";
import Text from "@docspace/components/text";
const StyledBodyContent = styled.div`
  .notification-container {
    display: grid;
    grid-template-columns: 540px 120px;
    margin-bottom: 24px;

    .toggle-btn {
      margin-left: auto;
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
`;
const SectionBodyContent = ({ t }) => {
  return (
    <StyledBodyContent>
      <StyledTextContent>
        <Text fontSize={"16px"} fontWeight={700}>
          {t("Badges")}
        </Text>
      </StyledTextContent>
      <div className="badges-container">
        <RoomsActionsContainer isEnable={true} t={t} />
      </div>
      <StyledTextContent>
        <Text fontSize={"16px"} fontWeight={700}>
          {t("Common:Email")}
        </Text>
      </StyledTextContent>
      <DailyFeedContainer isEnable={true} t={t} />
      <UsefulTipsContainer isEnable={true} t={t} />
    </StyledBodyContent>
  );
};

export default SectionBodyContent;
