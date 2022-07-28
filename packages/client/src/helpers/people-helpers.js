import { find, cloneDeep } from "lodash";
import {
  EmployeeActivationStatus,
  EmployeeStatus,
} from "@docspace/common/constants";
import { isAdmin } from "@docspace/common/utils";
import { id } from "PACKAGE_FILE";
//const { isAdmin } = utils;

export const getUserStatus = (user) => {
  if (
    user.status === EmployeeStatus.Active &&
    user.activationStatus === EmployeeActivationStatus.Activated
  ) {
    return "normal";
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

export const getUserRole = (user) => {
  if (user.isOwner) return "owner";
  else if (isAdmin(user, id))
    //TODO: Change to People Product Id const
    return "admin";
  //TODO: Need refactoring
  else if (user.isVisitor) return "guest";
  else return "user";
};

export const getUserContactsPattern = () => {
  return {
    contact: [
      {
        type: "mail",
        icon: "/static/images/mail.react.svg",
        link: "mailto:{0}",
      },
      {
        type: "phone",
        icon: "/images/phone.react.svg",
        link: "tel:{0}",
      },
      {
        type: "mobphone",
        icon: "/images/mobile.react.svg",
        link: "tel:{0}",
      },
      {
        type: "gmail",
        icon: "/images/gmail.react.svg",
        link: "mailto:{0}",
      },
      {
        type: "skype",
        icon: "/images/skype.react.svg",
        link: "skype:{0}?userinfo",
      },
      { type: "msn", icon: "/images/windows.msn.react.svg" },
      {
        type: "icq",
        icon: "/images/icq.react.svg",
        link: "https://www.icq.com/people/{0}",
      },
      { type: "jabber", icon: "/images/jabber.react.svg" },
      { type: "aim", icon: "/images/aim.react.svg" },
    ],
    social: [
      {
        type: "facebook",
        icon: "/images/share.facebook.react.svg",
        link: "https://facebook.com/{0}",
      },
      {
        type: "livejournal",
        icon: "/images/livejournal.react.svg",
        link: "https://{0}.livejournal.com",
      },
      {
        type: "myspace",
        icon: "/images/myspace.react.svg",
        link: "https://myspace.com/{0}",
      },
      {
        type: "twitter",
        icon: "/images/share.twitter.react.svg",
        link: "https://twitter.com/{0}",
      },
      {
        type: "blogger",
        icon: "/images/blogger.react.svg",
        link: "https://{0}.blogspot.com",
      },
      {
        type: "yahoo",
        icon: "/images/yahoo.react.svg",
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
