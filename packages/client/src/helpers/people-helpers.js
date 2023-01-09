import MailReactSvgUrl from "PUBLIC_DIR/images/mail.react.svg?url";
import { find, cloneDeep } from "lodash";
import {
  EmployeeActivationStatus,
  EmployeeStatus,
} from "@docspace/common/constants";

import PhoneIconUrl from "ASSETS_DIR/images/phone.react.svg?url";
import MobileIconUrl from "ASSETS_DIR/images/mobile.react.svg?url";
import GmailIconUrl from "ASSETS_DIR/images/gmail.react.svg?url";
import SkypeIconUrl from "ASSETS_DIR/images/skype.react.svg?url";
import MsnIconUrl from "ASSETS_DIR/images/windows.msn.react.svg?url";
import IcqIconUrl from "ASSETS_DIR/images/icq.react.svg?url";
import JabberIconUrl from "ASSETS_DIR/images/jabber.react.svg?url";
import AimIconUrl from "ASSETS_DIR/images/aim.react.svg?url";
import FacebookIconUrl from "ASSETS_DIR/images/share.facebook.react.svg?url";
import LivejournalIconUrl from "ASSETS_DIR/images/livejournal.react.svg?url";
import MyspaceIconUrl from "ASSETS_DIR/images/myspace.react.svg?url";
import TwitterIconUrl from "ASSETS_DIR/images/share.twitter.react.svg?url";
import BloggerIconUrl from "ASSETS_DIR/images/blogger.react.svg?url";
import YahooIconUrl from "ASSETS_DIR/images/yahoo.react.svg?url";

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

export function mapGroupsToGroupSelectorOptions(groups) {
  return groups.map((group) => {
    return {
      key: group.id,
      label: group.name,
      manager: group.manager,
      total: 0,
    };
  });
}

export function mapGroupSelectorOptionsToGroups(options) {
  return options.map((option) => {
    return {
      id: option.key,
      name: option.label,
      manager: option.manager,
    };
  });
}

export function filterGroupSelectorOptions(options, template) {
  return options.filter((option) => {
    return template ? option.label.indexOf(template) > -1 : true;
  });
}
