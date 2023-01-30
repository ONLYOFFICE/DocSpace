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
import toastr from "@docspace/components/toast/toastr";

const StyledBodyContent = styled.div`
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

let timerId = null;
const { Badges, RoomsActivity, DailyFeed, UsefulTips } = NotificationsType;

const SectionBodyContent = ({ t, ready, setSubscriptions }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [isContentLoaded, setIsContentLoaded] = useState(false);

  const getData = async () => {
    const requests = [
      getNotificationSubscription(Badges),
      getNotificationSubscription(RoomsActivity),
      getNotificationSubscription(DailyFeed),
      getNotificationSubscription(UsefulTips),
    ];

    try {
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
    } catch (e) {
      toastr.error(e);
    }
  };

  useEffect(async () => {
    timerId = setTimeout(() => {
      setIsLoading(true);
    }, 400);

    getData();
  }, []);

  const isLoadingContent = isLoading || !ready;

  if (!isLoading && !isContentLoaded) return <></>;

  const badgesContent = (
    <>
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
    </>
  );

  const emailContent = (
    <>
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
    </>
  );

  return (
    <StyledBodyContent>
      {badgesContent}
      {emailContent}
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
