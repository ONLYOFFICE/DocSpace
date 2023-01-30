import React, { useEffect, useState } from "react";
import UsefulTipsContainer from "./sub-components/UsefulTipsContainer";
import styled from "styled-components";
import RoomsActionsContainer from "./sub-components/RoomsActionsContainer";
import DailyFeedContainer from "./sub-components/DailyFeedContainer";
import RoomsActivityContainer from "./sub-components/RoomsActivityContainer";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import { NotificationsType } from "@docspace/common/constants";
import { getNotificationSubscription } from "@docspace/common/api/settings";
import Loaders from "@docspace/common/components/Loaders";

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

let timerId = null;
const SectionBodyContent = ({ t, ready, setSubscriptions }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [isContentLoaded, setIsContentLoaded] = useState(false);

  useEffect(async () => {
    timerId = setTimeout(() => {
      setIsLoading(true);
    }, 400);

    const requests = [
      getNotificationSubscription(NotificationsType.Badges),
      getNotificationSubscription(NotificationsType.RoomsActivity),
      getNotificationSubscription(NotificationsType.DailyFeed),
      getNotificationSubscription(NotificationsType.UsefulTips),
    ];

    const [badges, roomsActivity, dailyFeed, tips] = await Promise.all(
      requests
    );

    setSubscriptions(
      badges.isEnabled,
      roomsActivity.isEnabled,
      dailyFeed.isEnabled,
      tips.isEnabled
    );

    clearTimeout(timerId);
    timerId = null;

    setIsLoading(false);
    setIsContentLoaded(true);
  }, []);

  const isLoadingContent = isLoading || !ready;

  if (!isLoading && !isContentLoaded) return <></>;

  return (
    <StyledBodyContent>
      <StyledTextContent>
        {isLoadingContent ? (
          <Loaders.Rectangle height={"22px"} width={"57px"} />
        ) : (
          <Text fontSize={"16px"} fontWeight={700}>
            {t("Badges")}
          </Text>
        )}
      </StyledTextContent>
      <div className="badges-container">
        {isLoadingContent ? (
          <Loaders.Notifications />
        ) : (
          <RoomsActionsContainer t={t} />
        )}
      </div>
      <StyledTextContent>
        {isLoadingContent ? (
          <Loaders.Rectangle height={"22px"} width={"57px"} />
        ) : (
          <Text fontSize={"16px"} fontWeight={700}>
            {t("Common:Email")}
          </Text>
        )}
      </StyledTextContent>
      {isLoadingContent ? (
        <Loaders.Notifications count={3} />
      ) : (
        <>
          <RoomsActivityContainer t={t} />
          <DailyFeedContainer t={t} />
          <UsefulTipsContainer t={t} />{" "}
        </>
      )}
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
