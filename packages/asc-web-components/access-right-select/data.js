import {
  AccessNoneIcon,
  CheckRoundIcon,
  CommentIcon,
  CrownIcon,
  EyeIcon,
  PencilIcon,
  ReviewIcon,
} from "./svg";

export const data = [
  {
    key: "key1",
    title: "Room administrator",
    description: `Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do 
    eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.`,
    icon: CrownIcon,
  },
  {
    key: "key2",
    title: "Full access",
    description: `Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do 
      eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
      quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
      Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu 
      fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in 
      culpa qui officia deserunt mollit anim id est laborum.`,
    icon: CheckRoundIcon,
  },

  { key: "key3", isSeparator: true },
  {
    key: "key4",
    title: "Editing",
    description: `Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do 
    eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
    `,
    icon: PencilIcon,
  },
  {
    key: "key5",
    title: "Review",
    description: `Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do 
    eiusmod tempor incididunt ut labore et dolore magna aliqua.`,
    icon: ReviewIcon,
  },
  {
    key: "key6",
    title: "Comment",
    description: `Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do 
    eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
    Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu 
    fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in 
    culpa qui officia deserunt mollit anim id est laborum.`,
    icon: CommentIcon,
  },
  {
    key: "key7",
    title: "Read only",
    description: `Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do 
    eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
    Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu 
    fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in 
    culpa qui officia deserunt mollit anim id est laborum.`,
    icon: EyeIcon,
  },
  {
    key: "key8",
    title: "Deny access",
    description: "",
    icon: AccessNoneIcon,
  },
];

export const quota = [
  {
    key: "key1",
    accessRightKey: "key1",
    quota: "free",
    color: "#20D21F",
  },
  {
    key: "key2",
    accessRightKey: "key2",
    quota: "paid",
    color: "#EDC409",
  },
];
