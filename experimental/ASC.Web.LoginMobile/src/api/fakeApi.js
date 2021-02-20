export function login(login, pass) {
  console.log("Login Api", login, pass);
  return Promise.resolve();
}

export function join(portalName, email, firstName, lastName, pass) {
  console.log("Join Api", portalName, email, firstName, lastName, pass);
  return Promise.resolve();
}

export function  sendInstructionsToChangePassword(login) {
  console.log('Restore pwd', login)
  return Promise.resolve();
}
