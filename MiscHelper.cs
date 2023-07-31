using ReLogic.Graphics;
using Terraria.Graphics.Shaders;

namespace ShopLookup
{
    public static class MiscHelper
    {
        private static long id = 0;
        public static long ID => id;
        public static long NextID => id++;
        public static Rectangle NewRec(Vector2 start, Vector2 size) => NewRec(start.ToPoint(), size.ToPoint());
        public static Rectangle NewRec(Vector2 start, float width, float height)
        {
            return new Rectangle((int)start.X, (int)start.Y, (int)width, (int)height);
        }
        public static Rectangle NewRec(Point start, Point size)
        {
            return new Rectangle(start.X, start.Y, size.X, size.Y);
        }
        public static Rectangle NewRec(Point start, int width, int height)
        {
            return new Rectangle(start.X, start.Y, width, height);
        }
        public static Rectangle RecCenter(Vector2 center, int width, int height)
        {
            Point p = center.ToPoint();
            return new Rectangle(p.X - width / 2, p.Y - height / 2, width, height);
        }
        public static Rectangle RecCenter(Vector2 center, float width, float height) => RecCenter(center, (int)width, (int)height);

        public static int CalculatePhase(int value, int min, int max, int dis)
        {
            return value >= max ? (max - min) / dis : value <= min ? 0 : (value - min) / dis;
        }
        public static int Loop(int value, int min, int max)
        {
            int range = max - min + 1;
            if (value < min)
            {
                int diff = min - value;
                int mod = diff % range;
                return max - mod + 1;
            }
            if (value > max)
            {
                int diff = value - max;
                int mod = diff % range;
                return min + mod - 1;
            }
            return value;
        }
        public static float Loop(float value, float min, float max)
        {
            float range = max - min;
            if (value < min)
            {
                float diff = min - value;
                float mod = diff % range;
                return max - mod;
            }
            if (value > max)
            {
                float diff = value - max;
                float mod = diff % range;
                return min + mod;
            }
            return value;
        }
        public static Vector2 GetStringSize(this DynamicSpriteFont font, string text, Vector2 scale)
        {
            return font.MeasureString(text) * scale;
        }
        /// <summary>
        /// 花费文本
        /// </summary>
        public static void NewCombatTextValue(Rectangle location, float cost)
        {
            int platinum = (int)(cost / 1000000);
            int gold = (int)((cost - platinum * 1000000) / 10000);
            int silver = (int)((cost - platinum * 1000000 - gold * 10000) / 100);
            int copper = (int)(cost - platinum * 1000000 - gold * 10000 - silver * 100);
            Color color = Color.Orange;
            if (platinum > 0)
            {
                color = Color.White;
                CombatText.NewText(location, color, $"{platinum}铂金{gold}金{silver}银{copper}铜");
            }
            else if (gold > 0)
            {
                color = Color.Gold;
                CombatText.NewText(location, color, $"{gold}金{silver}银{copper}铜");
            }
            else if (silver > 0)
            {
                color = Color.Silver;
                CombatText.NewText(location, color, $"{silver}银{copper}铜");
            }
            else
            {
                CombatText.NewText(location, color, $"{copper}铜");
            }
        }
        public static bool InScreen(Vector2 pos)
        {
            float x = Main.screenPosition.X + Main.screenWidth;
            float y = Main.screenPosition.Y + Main.screenHeight;
            return pos.X < x && pos.X > Main.screenPosition.X && pos.Y < y && pos.Y > Main.screenPosition.Y;
        }
        public static void ReadyEffect(Player player)
        {
            SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
            for (int i = 0; i < 5; i++)
            {
                int num = Dust.NewDust(player.position, player.width, player.height, DustID.ManaRegeneration, 0f, 0f, 255, default, Main.rand.Next(20, 26) * 0.1f);
                Main.dust[num].noLight = true;
                Main.dust[num].noGravity = true;
                Main.dust[num].velocity *= 0.5f;
            }
        }
        public static void Yoraiz0rEye(Player player, int dusttype)
        {
            int num = 0;
            num += player.bodyFrame.Y / 56;
            if (num >= Main.OffsetsPlayerHeadgear.Length)
            {
                num = 0;
            }
            Vector2 vector = new Vector2(player.width / 2, player.height / 2) + Main.OffsetsPlayerHeadgear[num] + (player.MountedCenter - player.Center);
            player.sitting.GetSittingOffsetInfo(player, out Vector2 value, out float y);
            vector += value + new Vector2(0f, y);
            float num2 = -11.5f * player.gravDir;
            if (player.gravDir == -1f)
            {
                num2 -= 4f;
            }
            Vector2 vector2 = new Vector2(3 * player.direction - ((player.direction == 1) ? 1 : 0), num2) + Vector2.UnitY * player.gfxOffY + vector;
            Vector2 vector3 = new Vector2(3 * player.shadowDirection[1] - ((player.direction == 1) ? 1 : 0), num2) + vector;
            Vector2 vector4 = Vector2.Zero;
            if (player.mount.Active && player.mount.Cart)
            {
                int num3 = Math.Sign(player.velocity.X);
                if (num3 == 0)
                {
                    num3 = player.direction;
                }
                vector4 = new Vector2(MathHelper.Lerp(0f, -8f, player.fullRotation / 0.7853982f), MathHelper.Lerp(0f, 2f, Math.Abs(player.fullRotation / 0.7853982f))).RotatedBy(player.fullRotation, default);
                if (num3 == Math.Sign(player.fullRotation))
                {
                    vector4 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(player.fullRotation / 0.7853982f));
                }
            }
            if (player.fullRotation != 0f)
            {
                vector2 = vector2.RotatedBy(player.fullRotation, player.fullRotationOrigin);
                vector3 = vector3.RotatedBy(player.fullRotation, player.fullRotationOrigin);
            }
            float num4 = 0f;
            Vector2 vector5 = player.position + vector2 + vector4;
            Vector2 vector6 = player.oldPosition + vector3 + vector4;
            vector6.Y -= num4 / 2f;
            vector5.Y -= num4 / 2f;
            float num5 = 1f;
            switch (player.yoraiz0rEye % 10)
            {
                case 1:
                    return;
                case 2:
                    num5 = 0.5f;
                    break;
                case 3:
                    num5 = 0.625f;
                    break;
                case 4:
                    num5 = 0.75f;
                    break;
                case 5:
                    num5 = 0.875f;
                    break;
                case 6:
                    num5 = 1f;
                    break;
                case 7:
                    num5 = 1.1f;
                    break;
            }
            if (player.yoraiz0rEye < 7)
            {
                DelegateMethods.v3_1 = Main.hslToRgb(Main.rgbToHsl(player.eyeColor).X, 1f, 0.5f).ToVector3() * 0.5f * num5;
                if (player.velocity != Vector2.Zero)
                {
                    Utils.PlotTileLine(player.Center, player.Center + player.velocity * 2f, 4f, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
                }
                else
                {
                    Utils.PlotTileLine(player.Left, player.Right, 4f, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
                }
            }
            int num6 = (int)Vector2.Distance(vector5, vector6) / 3 + 1;
            if (Vector2.Distance(vector5, vector6) % 3f != 0f)
            {
                num6++;
            }
            for (float num7 = 1f; num7 <= num6; num7 += 1f)
            {
                Dust dust = Main.dust[Dust.NewDust(player.Center, 0, 0, DustID.TheDestroyer, 0f, 0f, 0, default, 1f)];
                dust.position = Vector2.Lerp(vector6, vector5, num7 / num6);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
                dust.customData = player;
                dust.scale = num5;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cYorai, player);
                dust.type = dusttype;
            }
        }
        /// <summary>
        /// 模式时间间隔区分
        /// </summary>
        public static int ModeNum(int commontime, int experttime, int mastertime)
        {
            return Main.expertMode ? (Main.masterMode ? mastertime : experttime) : commontime;
        }
        public static float ModeNum(float commontime, float experttime, float mastertime)
        {
            return Main.expertMode ? (Main.masterMode ? mastertime : experttime) : commontime;
        }
        /// <summary>
        /// 自动命名空间路径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name">是否自带类名</param>
        /// <returns></returns>
        public static string Path(this object type, bool name = false)
        {
            Type target = type.GetType();
            return target.Namespace.Replace(".", "/") + $"/{(name ? target.Name : null)}";
        }
        /// <summary>
        /// 上面那个的静态版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Path<T>(bool name = false)
        {
            Type target = typeof(T);
            return target.Namespace.Replace(".", "/") + $"/{(name ? target.Name : null)}";
        }
        /// <summary>
        /// 自动命名空间路径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Path(this object type, string fileName) => type.Path(false) + fileName;

        public static float Distance(this Entity entity, Entity target)
        {
            return entity.Distance(target.Center);
        }
        /*/// <summary>
        /// 本次伤害归零，传入攻击者扣除1dps
        /// </summary>
        public static void NoDamage(this NPC target, ref NPC.HitInfo info, Player attacker = null)
        {
            info.Damage = 1;
            info.HideCombatText = true;
            target.Cure(1, default);
            attacker?.addDPS(-1);
        }*/
        /// <summary>
        /// 非多人客户端
        /// </summary>
        /// <returns></returns>
        public static bool NetShoot() => Main.netMode != NetmodeID.MultiplayerClient;
        public static Texture2D Tex(this Entity entity)
        {
            if (entity is Projectile proj)
            {
                return TextureAssets.Projectile[proj.type].Value;
            }
            if (entity is NPC npc)
            {
                return TextureAssets.Npc[npc.type].Value;
            }
            if (entity is Item item)
            {
                return TextureAssets.Item[item.type].Value;
            }
            throw new Exception("不支持的Entity类型");
        }
        /// <summary>
        /// Buff基本设定
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="db">是否debuff</param>
        /// <param name="nurse">护士是否不可取消</param>
        /// <param name="display">是否不显示计时</param>
        /// <param name="noSave">退出后是否不保存</param>
        /// <param name="ex">若是db，是否专家以上延长时长</param>
        public static void SetBuff(this ModBuff buff, bool db, bool nurse, bool display, bool noSave, bool ex)
        {
            int type = buff.Type;
            Main.debuff[type] = db;
            BuffID.Sets.NurseCannotRemoveDebuff[type] = nurse;
            Main.buffNoTimeDisplay[type] = display;
            Main.buffNoSave[type] = noSave;
            BuffID.Sets.LongerExpertDebuff[type] = ex;
        }
        public static bool Precent(float n)
        {
            int d = 1;
            while (n % 1 != 0)
            {
                n *= 10;
                d *= 10;
            }
            return Main.rand.NextBool((int)n, d);
        }
        public static Rectangle ScaleRec(this Rectangle r, Vector2 scale)
        {
            r.X = (int)(r.X * scale.X);
            r.Y = (int)(r.Y * scale.Y);
            r.Width = (int)(r.Width * scale.X);
            r.Height = (int)(r.Height * scale.Y);
            return r;
        }
        /// <summary>
        /// 让矩形整体被矩阵缩放
        /// </summary>
        /// <param name="r"></param>
        /// <param name="matrix">万恶的缩放矩阵</param>
        /// <returns></returns>
        public static Rectangle ScaleRec(this Rectangle r, Matrix matrix) => ScaleRec(r, new Vector2(matrix.M11, matrix.M22));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="w">横向单个</param>
        /// <param name="h">纵向单个</param>
        /// <param name="offset"></param>
        /// <returns>从左到右从上到下的裁剪</returns>
        public static Rectangle[] Rec3x3(int w, int h, int offset = 2)
        {
            return new Rectangle[9]
            {
                new(0, 0, w, h),
                new(w + offset, 0, w, h),
                new((w + offset) * 2, 0, w, h),
                new(0, h + offset, w, h),
                new(w + offset, h + offset, w, h),
                new((w + offset) * 2, h + offset, w, h),
                new(0, (h + offset) * 2, w, h),
                new(w, (h + offset) * 2, w, h),
                new((w + offset) * 2, (h + offset) * 2, w, h),
            };
        }
        public static Color SetAlpha(this Color c, byte alpha)
        {
            c.A = alpha;
            return c;
        }
        public static Texture2D T2D(string path) => ModContent.Request<Texture2D>(path, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public static void DrawRec(SpriteBatch sb, Rectangle rec, float width, Color color, bool worldPos = true)
        {
            Vector2 scrPos = worldPos ? Vector2.Zero : Main.screenPosition;
            DrawLine(sb, rec.TopLeft() + scrPos, rec.TopRight() + scrPos, width, color);
            DrawLine(sb, rec.TopRight() + scrPos, rec.BottomRight() + scrPos, width, color);
            DrawLine(sb, rec.BottomRight() + scrPos, rec.BottomLeft() + scrPos, width, color);
            DrawLine(sb, rec.BottomLeft() + scrPos, rec.TopLeft() + scrPos, width, color);
        }
        /// <summary>
        /// 简易画线
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="wide">粗细</param>
        /// <param name="color">颜色</param>
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, float wide, Color color)
        {
            Texture2D texture = TextureAssets.MagicPixel.Value;
            Vector2 unit = end - start;
            spriteBatch.Draw(texture, start + unit / 2 - Main.screenPosition, new Rectangle(0, 0, 1, 1), color, unit.ToRotation() + MathHelper.PiOver2, new Vector2(0.5f, 0.5f), new Vector2(wide, unit.Length()), SpriteEffects.None, 0f);
        }
    }
}
