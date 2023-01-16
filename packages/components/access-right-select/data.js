import ActionsDocumentReactSvgUrl from "../../../public/images/actions.documents.react.svg?url";
import CheckReactSvgUrl from "../../../public/images/check.react.svg?url";
import AccessEditReactSvgUrl from "../../../public/images/access.edit.react.svg?url";
import AccessReviewReactSvgUrl from "../../../public/images/access.review.react.svg?url";
import AccessCommentReactSvgUrl from "../../../public/images/access.comment.react.svg?url";
import EyeReactSvgUrl from "../../../public/images/eye.react.svg?url";
import AccessNoneReactSvgUrl from "../../../public/images/access.none.react.svg?url";

export const data = [
  {
    key: "key1",
    label: "Room administrator",
    description: `Administration of rooms, archiving of rooms, inviting and managing users in rooms.`,
    icon: ActionsDocumentReactSvgUrl,
    quota: "free",
    color: "#20D21F",
  },
  {
    key: "key2",
    label: "Full access",
    description: `Edit, upload, create, view, download, delete files and folders.`,
    icon: CheckReactSvgUrl,
    quota: "paid",
    color: "#EDC409",
  },

  { key: "key3", isSeparator: true },
  {
    key: "key4",
    label: "Editing",
    description: `Editing, viewing, downloading files and folders, filling out forms.`,
    icon: AccessEditReactSvgUrl,
  },
  {
    key: "key5",
    label: "Review",
    description: `Reviewing, viewing, downloading files and folders, filling out forms.`,
    icon: AccessReviewReactSvgUrl,
  },
  {
    key: "key6",
    label: "Comment",
    description: `Commenting on files, viewing, downloading files and folders, filling out forms.`,
    icon: AccessCommentReactSvgUrl,
  },
  {
    key: "key7",
    label: "Read only",
    description: `Viewing, downloading files and folders, filling out forms.`,
    icon: EyeReactSvgUrl,
  },
  {
    key: "key8",
    label: "Deny access",
    description: "",
    icon: AccessNoneReactSvgUrl,
  },
];
