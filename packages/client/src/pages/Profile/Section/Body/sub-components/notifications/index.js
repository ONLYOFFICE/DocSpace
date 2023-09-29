import { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import { NotificationsType } from "@docspace/common/constants";
import { getNotificationSubscription } from "@docspace/common/api/settings";
import Loaders from "@docspace/common/components/Loaders";
import toastr from "@docspace/components/toast/toastr";

import UsefulTipsContainer from "./sub-components/UsefulTipsContainer";
import RoomsActionsContainer from "./sub-components/RoomsActionsContainer";
import DailyFeedContainer from "./sub-components/DailyFeedContainer";
import RoomsActivityContainer from "./sub-components/RoomsActivityContainer";
import { StyledSectionBodyContent, StyledTextContent } from "./StyledComponent";

const { Badges, RoomsActivity, DailyFeed, UsefulTips } = NotificationsType;

const Notifications = ({ setSubscriptions, isFirstSubscriptionsLoad }) => {
  const { t, ready } = useTranslation("Notifications");

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

      setIsLoading(false);
      setIsContentLoaded(true);
    } catch (e) {
      toastr.error(e);
    }
  };

  useEffect(() => {
    if (isFirstSubscriptionsLoad) {
      setIsLoading(true);
      getData();
    }
  }, []);

  const isLoadingContent = isLoading || !ready;

  //if (!isLoading && !isContentLoaded) return <></>;

  const textProps = {
    fontSize: "13px",
    fontWeight: "600",
    noSelect: true,
  };
  const textDescriptionsProps = {
    fontSize: "13px",
    fontWeight: "400",
    className: "notification-container_description",
  };

  const badgesContent = (
    <>
      <StyledTextContent>
        {isLoadingContent ? (
          <Loaders.Rectangle height={"22px"} width={"57px"} />
        ) : (
          <Text fontSize={"14px"} fontWeight={600}>
            {t("Badges")}
          </Text>
        )}
      </StyledTextContent>
      <div className="badges-container">
        {isLoadingContent ? (
          <Loaders.Notifications />
        ) : (
          <RoomsActionsContainer
            t={t}
            textProps={textProps}
            textDescriptionsProps={textDescriptionsProps}
          />
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
          <Text fontSize={"14px"} fontWeight={600} className="email-title">
            {t("Common:Email")}
          </Text>
        )}
      </StyledTextContent>
      {isLoadingContent ? (
        <Loaders.Notifications count={3} />
      ) : (
        <div className="badges-container">
          <RoomsActivityContainer
            t={t}
            textProps={textProps}
            textDescriptionsProps={textDescriptionsProps}
          />
          <DailyFeedContainer
            t={t}
            textProps={textProps}
            textDescriptionsProps={textDescriptionsProps}
          />
          <UsefulTipsContainer
            t={t}
            textProps={textProps}
            textDescriptionsProps={textDescriptionsProps}
          />
        </div>
      )}
    </>
  );

  return (
    <StyledSectionBodyContent>
      {badgesContent}
      {emailContent}
    </StyledSectionBodyContent>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { setSubscriptions, isFirstSubscriptionsLoad } = targetUserStore;

  return {
    setSubscriptions,
    isFirstSubscriptionsLoad,
  };
})(observer(Notifications));
