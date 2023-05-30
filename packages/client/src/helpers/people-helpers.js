import React from "react";
import { Trans } from "react-i18next";
import MailReactSvgUrl from "PUBLIC_DIR/images/mail.react.svg?url";
import { find, cloneDeep } from "lodash";
import {
  EmployeeActivationStatus,
  EmployeeStatus,
} from "@docspace/common/constants";

import PhoneIconUrl from "PUBLIC_DIR/images/phone.react.svg?url";
import MobileIconUrl from "PUBLIC_DIR/images/mobile.react.svg?url";
import GmailIconUrl from "PUBLIC_DIR/images/gmail.react.svg?url";
import SkypeIconUrl from "PUBLIC_DIR/images/skype.react.svg?url";
import MsnIconUrl from "PUBLIC_DIR/images/windows.msn.react.svg?url";
import IcqIconUrl from "PUBLIC_DIR/images/icq.react.svg?url";
import JabberIconUrl from "PUBLIC_DIR/images/jabber.react.svg?url";
import AimIconUrl from "PUBLIC_DIR/images/aim.react.svg?url";
import FacebookIconUrl from "PUBLIC_DIR/images/share.facebook.react.svg?url";
import LivejournalIconUrl from "PUBLIC_DIR/images/livejournal.react.svg?url";
import MyspaceIconUrl from "PUBLIC_DIR/images/myspace.react.svg?url";
import TwitterIconUrl from "PUBLIC_DIR/images/share.twitter.react.svg?url";
import BloggerIconUrl from "PUBLIC_DIR/images/blogger.react.svg?url";
import YahooIconUrl from "PUBLIC_DIR/images/yahoo.react.svg?url";
import toastr from "@docspace/components/toast/toastr";

export const getUserStatus = (user) => {
  if (
    user.status === EmployeeStatus.Active &&
    user.activationStatus === EmployeeActivationStatus.Activated
  ) {
    return "active";
  } else if (
    user.status === EmployeeStatus.Active &&
    user.activationStatus === EmployeeActivationStatus.Pending
  ) {
    return "pending";
  } else if (user.status === EmployeeStatus.Disabled) {
    return "disabled";
  } else {
    return "unknown";
  }
};

export const getUserContactsPattern = () => {
  return {
    contact: [
      {
        type: "mail",
        icon: MailReactSvgUrl,
        link: "mailto:{0}",
      },
      {
        type: "phone",
        icon: PhoneIconUrl,
        link: "tel:{0}",
      },
      {
        type: "mobphone",
        icon: MobileIconUrl,
        link: "tel:{0}",
      },
      {
        type: "gmail",
        icon: GmailIconUrl,
        link: "mailto:{0}",
      },
      {
        type: "skype",
        icon: SkypeIconUrl,
        link: "skype:{0}?userinfo",
      },
      { type: "msn", icon: MsnIconUrl },
      {
        type: "icq",
        icon: IcqIconUrl,
        link: "https://www.icq.com/people/{0}",
      },
      { type: "jabber", icon: JabberIconUrl },
      { type: "aim", icon: AimIconUrl },
    ],
    social: [
      {
        type: "facebook",
        icon: FacebookIconUrl,
        link: "https://facebook.com/{0}",
      },
      {
        type: "livejournal",
        icon: LivejournalIconUrl,
        link: "https://{0}.livejournal.com",
      },
      {
        type: "myspace",
        icon: MyspaceIconUrl,
        link: "https://myspace.com/{0}",
      },
      {
        type: "twitter",
        icon: TwitterIconUrl,
        link: "https://twitter.com/{0}",
      },
      {
        type: "blogger",
        icon: BloggerIconUrl,
        link: "https://{0}.blogspot.com",
      },
      {
        type: "yahoo",
        icon: YahooIconUrl,
        link: "mailto:{0}@yahoo.com",
      },
    ],
  };
};

export const getUserContacts = (contacts) => {
  const mapContacts = (a, b) => {
    return a
      .map((a) => ({ ...a, ...b.find(({ type }) => type === a.type) }))
      .filter((c) => c.icon);
  };

  const info = {};
  const pattern = getUserContactsPattern();

  info.contact = mapContacts(contacts, pattern.contact);
  info.social = mapContacts(contacts, pattern.social);

  return info;
};

export function getSelectedGroup(groups, selectedGroupId) {
  return find(groups, (group) => group.id === selectedGroupId);
}

export function toEmployeeWrapper(profile) {
  const emptyData = {
    id: "",
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    birthday: "",
    sex: "male",
    workFrom: "",
    location: "",
    title: "",
    groups: [],
    notes: "",
    contacts: [],
  };

  return cloneDeep({ ...emptyData, ...profile });
}

export const showEmailActivationToast = (email) => {
  //console.log("showEmailActivationToast", { email });
  toastr.success(
    <Trans i18nKey="MessageEmailActivationInstuctionsSentOnEmail" ns="People">
      The email activation instructions have been sent to the
      <strong>{{ email }}</strong> email address
    </Trans>
  );
};
