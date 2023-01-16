import AccessNoneIconUrl from "../../../public/images/access.none.react.svg?url";
import CheckRoundIconUrl from "../../../public/images/check.react.svg?url";
import CommentIconUrl from "../../../public/images/access.comment.react.svg?url";
import CrownIconUrl from "../../../public/images/access.edit.react.svg?url";
import EyeIconUrl from "../../../public/images/eye.react.svg?url";
import PencilIconUrl from "../../../public/images/access.edit.react.svg?url";
import ReviewIconUrl from "../../../public/images/access.review.react.svg?url";

export const data = [
  {
    key: "key1",
    label: "Room administrator",
    description: `Administration of rooms, archiving of rooms, inviting and managing users in rooms.`,
    icon: CrownIconUrl,
    quota: "free",
    color: "#20D21F",
  },
  {
    key: "key2",
    label: "Full access",
    description: `Edit, upload, create, view, download, delete files and folders.`,
    icon: CheckRoundIconUrl,
    quota: "paid",
    color: "#EDC409",
  },

  { key: "key3", isSeparator: true },
  {
    key: "key4",
    label: "Editing",
    description: `Editing, viewing, downloading files and folders, filling out forms.`,
    icon: PencilIconUrl,
  },
  {
    key: "key5",
    label: "Review",
    description: `Reviewing, viewing, downloading files and folders, filling out forms.`,
    icon: ReviewIconUrl,
  },
  {
    key: "key6",
    label: "Comment",
    description: `Commenting on files, viewing, downloading files and folders, filling out forms.`,
    icon: CommentIconUrl,
  },
  {
    key: "key7",
    label: "Read only",
    description: `Viewing, downloading files and folders, filling out forms.`,
    icon: EyeIconUrl,
  },
  {
    key: "key8",
    label: "Deny access",
    description: "",
    icon: AccessNoneIconUrl,
  },
];
