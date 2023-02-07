export default class ObjectUtils {
  static getJSXElement(obj, ...params) {
    return this.isFunction(obj) ? obj(...params) : obj;
  }
}
