import React, { useEffect } from "react";
import UsefulTipsContainer from "./sub-components/UsefulTipsContainer";
import styled from "styled-components";
import RoomsActionsContainer from "./sub-components/RoomsActionsContainer";
import DailyFeedContainer from "./sub-components/DailyFeedContainer";
import RoomsActivityContainer from "./sub-components/RoomsActivityContainer";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";

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
const SectionBodyContent = ({ t, setSubscriptions }) => {
  useEffect(async () => {
    await setSubscriptions();
  }, []);

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
      <RoomsActivityContainer isEnable={true} t={t} />
      <DailyFeedContainer isEnable={true} t={t} />
      <UsefulTipsContainer t={t} />
    </StyledBodyContent>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { setSubscriptions } = targetUserStore;

  return {
    setSubscriptions,
  };
})(observer(SectionBodyContent));
