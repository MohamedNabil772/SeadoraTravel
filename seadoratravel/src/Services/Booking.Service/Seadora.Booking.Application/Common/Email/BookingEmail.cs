using System;
using System.Collections.Generic;
using System.Text;
using Seadora.Booking.Domain.Entities;

namespace Seadora.Booking.Application.Common.Email;

public static class BookingEmail
{
    // Color Palette Tokens
    private const string BrandNavy = "#062D4D";
    private const string BrandDark = "#041829";
    private const string GoldPrimary = "#D4AF37";
    private const string GoldDark = "#B45309";
    private const string GoldAmber = "#D97706";
    private const string BackgroundLight = "#F8FAFC";
    private const string CardWhite = "#FFFFFF";
    private const string CardSubtle = "#F1F5F9";
    private const string TextCharcoal = "#0F172A";
    private const string TextBody = "#1E293B";
    private const string TextLabel = "#475569";
    private const string BorderStrong = "#CBD5E1";
    private const string BorderGold = "#D4AF37";
    private const string GreenEmerald = "#047857";
    private const string GreenBorder = "#059669";
    private const string GreenBg = "#ECFDF5";
    private const string AmberBg = "#FEF3C7";
    private const string AmberBorder = "#F59E0B";
    private const string AmberText = "#92400E";
    private const string LogoUrl = "https://seadoratravel.com/logo-emblem.png";
    private const string WhatsAppUrl = ContactChannels.WhatsAppUrl;
    private const string WebsiteUrl = ContactChannels.WebsiteUrl;

    private class EmailStrings
    {
        public string HeaderSubtitle { get; set; } = "";
        public string PreheaderReceived { get; set; } = "";
        public string StatusReceived { get; set; } = "";
        public string TitleReceived { get; set; } = "";
        public string Greeting { get; set; } = "";
        public string ReceivedIntro { get; set; } = "";
        public string PreheaderConfirmed { get; set; } = "";
        public string StatusConfirmed { get; set; } = "";
        public string TitleConfirmed { get; set; } = "";
        public string ConfirmedIntro { get; set; } = "";
        public string VoucherRef { get; set; } = "";
        public string OfficialVoucher { get; set; } = "";
        public string TourDate { get; set; } = "";
        public string PickupWindow { get; set; } = "";
        public string Guests { get; set; } = "";
        public string TotalAmount { get; set; } = "";
        public string PickupLocation { get; set; } = "";
        public string WhatNextTitle { get; set; } = "";
        public string WhatNextDesc { get; set; } = "";
        public string RemindersTitle { get; set; } = "";
        public string Reminder1 { get; set; } = "";
        public string Reminder2 { get; set; } = "";
        public string Reminder3 { get; set; } = "";
        public string FlexibilityTitle { get; set; } = "";
        public string FlexibilityDesc { get; set; } = "";
        public string ConciergeTitle { get; set; } = "";
        public string ConciergeDesc { get; set; } = "";
        public string WhatsAppCta { get; set; } = "";
        public string FooterMarina { get; set; } = "";
        public string FooterRights { get; set; } = "";
        public string DefaultExperience { get; set; } = "";
        public string DateTbd { get; set; } = "";
        public string PickupTbd { get; set; } = "";
        public string SelectedAddonsLabel { get; set; } = "";
        public string GuestsBreakdownLabel { get; set; } = "";
    }

    private static readonly Dictionary<string, EmailStrings> Localizations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = new EmailStrings
        {
            HeaderSubtitle = "Luxury Red Sea Concierge & Private Journeys",
            PreheaderReceived = "Your VIP reservation request has been received and is being prepared.",
            StatusReceived = "Reservation Request Received",
            TitleReceived = "Booking Received — Seadora Travel",
            Greeting = "Dear",
            ReceivedIntro = "Thank you for selecting <strong>Seadora Luxury Travel</strong>. We have received your reservation request and our VIP operations team in Hurghada is currently reviewing your schedule to ensure every detail meets our five-star standards.",
            PreheaderConfirmed = "Your VIP Booking is CONFIRMED! View your official travel voucher inside.",
            StatusConfirmed = "Officially Confirmed",
            TitleConfirmed = "Booking Confirmed — Seadora Travel",
            ConfirmedIntro = "We are delighted to confirm your luxury experience with <strong>Seadora Travel</strong>. Your private vehicle, licensed guide, and bespoke arrangements are fully secured. Please keep this voucher accessible on your phone.",
            VoucherRef = "VOUCHER REFERENCE",
            OfficialVoucher = "OFFICIAL TRAVEL VOUCHER",
            TourDate = "Date of Experience",
            PickupWindow = "Pickup Window",
            Guests = "Party Size",
            TotalAmount = "Total Amount",
            PickupLocation = "Meeting / Pickup Location",
            WhatNextTitle = "What Happens Next?",
            WhatNextDesc = "Our concierge will finalize details with your private chauffeur and crew. You will receive real-time updates and your driver's contact info via WhatsApp prior to departure.",
            RemindersTitle = "Important Reminders for Tour Day",
            Reminder1 = "Please carry a physical passport/ID or a clear photo on your mobile phone.",
            Reminder2 = "Please be present in your hotel lobby 10 minutes prior to the pickup window.",
            Reminder3 = "Comfortable resort wear, sunscreen, and sunglasses are highly recommended.",
            FlexibilityTitle = "Guaranteed Flexibility",
            FlexibilityDesc = "Free cancellation and 100% full refund available up to 48 hours prior to departure. Need date adjustments? Message our concierge anytime.",
            ConciergeTitle = "Dedicated 24/7 VIP Concierge",
            ConciergeDesc = "Our local Hurghada travel specialists are on standby to accommodate any bespoke requests or dietary preferences.",
            WhatsAppCta = "💬 Chat with Concierge on WhatsApp",
            FooterMarina = "Hurghada Marina, Red Sea Governorate, Egypt",
            FooterRights = "All rights reserved. • High-End Hospitality by SEADORA",
            DefaultExperience = "Private VIP Experience",
            DateTbd = "Date on Request",
            PickupTbd = "To be confirmed by Concierge",
            SelectedAddonsLabel = "Selected Upgrades & Add-ons",
            GuestsBreakdownLabel = "Registered VIP Guests"
        },
        ["de"] = new EmailStrings
        {
            HeaderSubtitle = "Luxus Rotes Meer Concierge & Private Reisen",
            PreheaderReceived = "Ihre VIP-Reservierungsanfrage ist eingegangen und wird vorbereitet.",
            StatusReceived = "Reservierungsanfrage Eingegangen",
            TitleReceived = "Buchung Eingegangen — Seadora Travel",
            Greeting = "Sehr geehrte(r)",
            ReceivedIntro = "Vielen Dank, dass Sie sich für <strong>Seadora Luxury Travel</strong> entschieden haben. Wir haben Ihre Buchungsanfrage erhalten. Unser VIP-Team in Hurghada prüft derzeit Ihren Zeitplan, um erstklassige Exzellenz zu gewährleisten.",
            PreheaderConfirmed = "Ihre VIP-Buchung ist BESTÄTIGT! Ihr offizieller Reisegutschein im Anhang.",
            StatusConfirmed = "Offiziell Bestätigt",
            TitleConfirmed = "Buchung Bestätigt — Seadora Travel",
            ConfirmedIntro = "Wir freuen uns sehr, Ihr Luxuserlebnis mit <strong>Seadora Travel</strong> zu bestätigen. Ihr Privatfahrzeug, Reiseleiter und alle VIP-Arrangements sind gesichert. Bitte halten Sie diesen Voucher bereit.",
            VoucherRef = "VOUCHER-REFERENZ",
            OfficialVoucher = "OFFIZIELLER REISEGUTSCHEIN",
            TourDate = "Datum des Erlebnisses",
            PickupWindow = "Abholzeitfenster",
            Guests = "Teilnehmerzahl",
            TotalAmount = "Gesamtbetrag",
            PickupLocation = "Treffpunkt / Abholort",
            WhatNextTitle = "Was passiert als Nächstes?",
            WhatNextDesc = "Unser Concierge finalisiert alle Details mit Ihrem Chauffeur. Sie erhalten rechtzeitig vor der Abfahrt alle Fahrerdaten und Live-Updates via WhatsApp.",
            RemindersTitle = "Wichtige Hinweise für den Tourtag",
            Reminder1 = "Bitte führen Sie Ihren Reisepass/Ausweis oder ein klares Foto auf dem Smartphone mit.",
            Reminder2 = "Bitte finden Sie sich 10 Minuten vor der geplanten Zeit in der Hotellobby ein.",
            Reminder3 = "Bequeme Kleidung, Sonnenschutz und Sonnenbrille werden wärmstens empfohlen.",
            FlexibilityTitle = "Garantierte Flexibilität",
            FlexibilityDesc = "Kostenlose Stornierung und 100% Rückerstattung bis zu 48 Stunden vor Beginn. Datumsänderungen sind jederzeit über unseren Concierge möglich.",
            ConciergeTitle = "Dedizierter 24/7 VIP-Concierge",
            ConciergeDesc = "Unsere Reisespezialisten in Hurghada stehen Ihnen jederzeit für Sonderwünsche zur Verfügung.",
            WhatsAppCta = "💬 Mit Concierge auf WhatsApp chatten",
            FooterMarina = "Hurghada Marina, Rotes Meer, Ägypten",
            FooterRights = "Alle Rechte vorbehalten. • Exzellenter Service von SEADORA",
            DefaultExperience = "Privates VIP-Erlebnis",
            DateTbd = "Datum auf Anfrage",
            PickupTbd = "Wird vom Concierge bestätigt",
            SelectedAddonsLabel = "Gewählte Upgrades & Extras",
            GuestsBreakdownLabel = "Registrierte VIP-Gäste"
        },
        ["it"] = new EmailStrings
        {
            HeaderSubtitle = "Concierge di Lusso del Mar Rosso & Viaggi Privati",
            PreheaderReceived = "La tua richiesta di prenotazione VIP è stata ricevuta.",
            StatusReceived = "Richiesta Ricevuta",
            TitleReceived = "Prenotazione Ricevuta — Seadora Travel",
            Greeting = "Gentile",
            ReceivedIntro = "Grazie per aver scelto <strong>Seadora Luxury Travel</strong>. Abbiamo ricevuto la tua richiesta di prenotazione e il nostro team VIP a Hurghada sta organizzando ogni dettaglio secondo i più alti standard.",
            PreheaderConfirmed = "La tua prenotazione VIP è CONFERMATA! Visualizza il tuo voucher ufficiale.",
            StatusConfirmed = "Ufficialmente Confermata",
            TitleConfirmed = "Prenotazione Confermata — Seadora Travel",
            ConfirmedIntro = "Siamo lieti di confermare la tua esperienza di lusso con <strong>Seadora Travel</strong>. Il tuo veicolo privato, la guida e tutti i servizi sono confermati.",
            VoucherRef = "RIFERIMENTO VOUCHER",
            OfficialVoucher = "VOUCHER DI VIAGGIO UFFICIALE",
            TourDate = "Data dell'Esperienza",
            PickupWindow = "Orario di Prelievo",
            Guests = "Numero Ospiti",
            TotalAmount = "Importo Totale",
            PickupLocation = "Luogo di Prelievo / Hotel",
            WhatNextTitle = "Cosa Succede Ora?",
            WhatNextDesc = "Il nostro concierge coordinerà i dettagli finali con l'autista privato. Riceverai aggiornamenti e contatti diretti via WhatsApp prima della partenza.",
            RemindersTitle = "Promemoria Importanti per la Giornata",
            Reminder1 = "Porta con te il passaporto/documento o una foto nitida sul cellulare.",
            Reminder2 = "Presentati nella hall dell'hotel 10 minuti prima dell'orario concordato.",
            Reminder3 = "Si consigliano abbigliamento comodo, protezione solare e occhiali da sole.",
            FlexibilityTitle = "Flessibilità Garantita",
            FlexibilityDesc = "Cancellazione gratuita con rimborso al 100% fino a 48 ore prima della partenza.",
            ConciergeTitle = "Concierge VIP Dedicato 24/7",
            ConciergeDesc = "I nostri specialisti di Hurghada sono sempre a tua completa disposizione.",
            WhatsAppCta = "💬 Chatta con il Concierge su WhatsApp",
            FooterMarina = "Marina di Hurghada, Mar Rosso, Egitto",
            FooterRights = "Tutti i diritti riservati. • Eccellenza SEADORA",
            DefaultExperience = "Esperienza Privata VIP",
            DateTbd = "Data su richiesta",
            PickupTbd = "Da confermare dal Concierge",
            SelectedAddonsLabel = "Servizi Aggiuntivi Selezionati",
            GuestsBreakdownLabel = "Ospiti VIP Registrati"
        },
        ["fr"] = new EmailStrings
        {
            HeaderSubtitle = "Conciergerie de Luxe Mer Rouge & Voyages Privés",
            PreheaderReceived = "Votre demande de réservation VIP a bien été reçue.",
            StatusReceived = "Demande Reçue",
            TitleReceived = "Réservation Reçue — Seadora Travel",
            Greeting = "Cher(ère)",
            ReceivedIntro = "Merci d'avoir choisi <strong>Seadora Luxury Travel</strong>. Nous avons bien reçu votre demande de réservation. Notre équipe VIP à Hurghada prépare minutieusement votre voyage d'exception.",
            PreheaderConfirmed = "Votre réservation VIP est CONFIRMÉE ! Consultez votre bon officiel.",
            StatusConfirmed = "Officiellement Confirmée",
            TitleConfirmed = "Réservation Confirmée — Seadora Travel",
            ConfirmedIntro = "Nous sommes ravis de confirmer votre expérience de prestige avec <strong>Seadora Travel</strong>. Votre véhicule privé et votre guide dédié sont réservés.",
            VoucherRef = "RÉFÉRENCE DU BON",
            OfficialVoucher = "BON DE VOYAGE OFFICIEL",
            TourDate = "Date de l'Expérience",
            PickupWindow = "Créneau de Prise en Charge",
            Guests = "Nombre de Voyageurs",
            TotalAmount = "Montant Total",
            PickupLocation = "Lieu de Prise en Charge / Hôtel",
            WhatNextTitle = "Prochaines Étapes",
            WhatNextDesc = "Notre conciergerie finalise les détails avec votre chauffeur privé. Vous recevrez toutes les coordonnées directes par WhatsApp avant le départ.",
            RemindersTitle = "Conseils Importants pour le Jour J",
            Reminder1 = "Veuillez vous munir de votre passeport ou d'une photo claire sur smartphone.",
            Reminder2 = "Rendez-vous dans le hall de votre hôtel 10 minutes avant l'heure prévue.",
            Reminder3 = "Tenue décontractée, crème solaire et lunettes de soleil sont vivement conseillées.",
            FlexibilityTitle = "Flexibilité Garantie",
            FlexibilityDesc = "Annulation gratuite avec remboursement à 100% jusqu'à 48h avant le départ.",
            ConciergeTitle = "Conciergerie VIP Dédiée 24/7",
            ConciergeDesc = "Nos spécialistes à Hurghada restent à votre entière disposition.",
            WhatsAppCta = "💬 Discuter avec la Conciergerie sur WhatsApp",
            FooterMarina = "Marina d'Hurghada, Mer Rouge, Égypte",
            FooterRights = "Tous droits réservés. • Prestations d'Excellence par SEADORA",
            DefaultExperience = "Expérience VIP Privée",
            DateTbd = "Date sur demande",
            PickupTbd = "À confirmer par la conciergerie",
            SelectedAddonsLabel = "Options & Suppléments",
            GuestsBreakdownLabel = "Voyageurs VIP Enregistrés"
        },
        ["ru"] = new EmailStrings
        {
            HeaderSubtitle = "VIP Консьерж-сервис на Красном Море и Индивидуальные Туры",
            PreheaderReceived = "Ваш VIP-запрос на бронирование получен и обрабатывается.",
            StatusReceived = "Запрос на Бронирование Получен",
            TitleReceived = "Бронирование Получено — Seadora Travel",
            Greeting = "Уважаемый(ая)",
            ReceivedIntro = "Благодарим вас за выбор <strong>Seadora Luxury Travel</strong>. Мы получили ваш запрос на бронирование. Наша VIP-команда в Хургаде уже готовит поездку по высочайшим стандартам пятизвездочного сервиса.",
            PreheaderConfirmed = "Ваше VIP-бронирование ПОДТВЕРЖДЕНО! Ваш официальный ваучер внутри.",
            StatusConfirmed = "Официально Подтверждено",
            TitleConfirmed = "Бронирование Подтверждено — Seadora Travel",
            ConfirmedIntro = "Мы рады подтвердить ваше эксклюзивное путешествие с <strong>Seadora Travel</strong>. Индивидуальный комфортабельный трансфер и персональный гид закреплены за вами.",
            VoucherRef = "НОМЕР ВАУЧЕРА",
            OfficialVoucher = "ОФИЦИАЛЬНЫЙ ТУРИСТИЧЕСКИЙ ВАУЧЕР",
            TourDate = "Дата Поездки",
            PickupWindow = "Время Трансфера из Отеля",
            Guests = "Количество Гостей",
            TotalAmount = "Итоговая Сумма",
            PickupLocation = "Место Сбора / Отель",
            WhatNextTitle = "Что Происходит Дальше?",
            WhatNextDesc = "Консьерж согласует все детали с водителем и экипажем. Перед выездом вы получите контакты водителя и уведомление в WhatsApp.",
            RemindersTitle = "Важные Рекомендации на День Экскурсии",
            Reminder1 = "Возьмите с собой оригинал паспорта или четкое фото на смартфоне.",
            Reminder2 = "Пожалуйста, ожидайте в лобби отеля за 10 минут до назначенного времени трансфера.",
            Reminder3 = "Рекомендуем удобную обувь, солнцезащитные очки и крем от солнца.",
            FlexibilityTitle = "Гарантия Гибкости",
            FlexibilityDesc = "Бесплатная отмена и 100% возврат средств за 48 часов до выезда. Изменение даты доступно в любое время через консьержа.",
            ConciergeTitle = "Персональный VIP-Консьерж 24/7",
            ConciergeDesc = "Наши специалисты в Хургаде круглосуточно на связи для любых пожеланий.",
            WhatsAppCta = "💬 Написать Консьержу в WhatsApp",
            FooterMarina = "Марина Хургада, провинция Красное Море, Египет",
            FooterRights = "Все права защищены. • Премиальный сервис SEADORA",
            DefaultExperience = "Индивидуальный VIP Тур",
            DateTbd = "Дата по согласованию",
            PickupTbd = "Будет уточнено консьержем",
            SelectedAddonsLabel = "Выбранные Дополнительные Опции",
            GuestsBreakdownLabel = "Список Зарегистрированных Гостей"
        }
    };

    private static EmailStrings GetStrings(string? lang)
    {
        if (string.IsNullOrWhiteSpace(lang)) return Localizations["en"];
        var key = lang.Trim().ToLowerInvariant();
        return Localizations.TryGetValue(key, out var s) ? s : Localizations["en"];
    }

    private static string GetEmailHeader(EmailStrings s, string preheader, string statusPill, string title, bool isConfirmed = false)
    {
        var badgeBg = isConfirmed ? GreenBg : AmberBg;
        var badgeBorder = isConfirmed ? GreenBorder : AmberBorder;
        var badgeColor = isConfirmed ? GreenEmerald : AmberText;

        return $@"
        <!DOCTYPE html>
        <html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <meta name='color-scheme' content='light dark'>
            <meta name='supported-color-schemes' content='light dark'>
            <title>{title}</title>
            <style>
                :root {{
                    color-scheme: light dark;
                    supported-color-schemes: light dark;
                }}
                @import url('https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,600;0,700;0,800;1,400&family=Jost:wght@400;500;600;700;800&display=swap');
                
                body {{
                    margin: 0;
                    padding: 0;
                    background-color: {BackgroundLight};
                    font-family: 'Jost', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                    -webkit-font-smoothing: antialiased;
                    color: {TextCharcoal};
                }}
                table {{ border-collapse: separate; }}
                a {{ text-decoration: none; }}

                /* High Specificity Class Styles */
                .brand-title {{ font-family: 'Playfair Display', Georgia, serif; font-size: 28px; font-weight: 800; color: {BrandNavy}; letter-spacing: 4px; text-transform: uppercase; }}
                .heading-dark {{ font-family: 'Playfair Display', Georgia, serif; font-size: 23px; font-weight: 800; color: {BrandNavy}; }}
                .text-dark {{ color: {TextCharcoal}; font-size: 15px; line-height: 1.7; }}
                .label-dark {{ color: {TextLabel}; font-size: 11px; font-weight: 800; text-transform: uppercase; letter-spacing: 1px; }}
                .value-dark {{ color: {BrandNavy}; font-size: 16px; font-weight: 800; }}
                .price-gold {{ color: {GoldAmber}; font-size: 22px; font-weight: 800; }}

                /* Gmail iOS & Android Dark Mode Text Inversion Hack */
                u + .body .gmail-blend-screen {{ background: #000; mix-blend-mode: screen; }}
                u + .body .gmail-blend-difference {{ background: #000; mix-blend-mode: difference; }}

                /* Outlook Web Force */
                [data-ogsb] .email-wrapper {{ background-color: {BackgroundLight} !important; }}
                [data-ogsb] .white-card {{ background-color: #FFFFFF !important; }}
                [data-ogsc] .brand-title {{ color: {BrandNavy} !important; }}
                [data-ogsc] .heading-dark {{ color: {BrandNavy} !important; }}
                [data-ogsc] .text-dark {{ color: {TextCharcoal} !important; }}
                [data-ogsc] .value-dark {{ color: {BrandNavy} !important; }}

                /* Dark Mode Media Query for Apple Mail & Modern Clients */
                @media (prefers-color-scheme: dark) {{
                    body, .email-wrapper {{ background-color: #0A1118 !important; }}
                    .white-card {{ background-color: #111C28 !important; border-color: #D4AF37 !important; }}
                    .card-subtle {{ background-color: #0D1722 !important; border-color: #1E2D3D !important; }}
                    .card-highlight {{ background-color: #0F1D2C !important; border-color: #1E2D3D !important; }}
                    .brand-title, .heading-dark, .value-dark {{ color: #FFFFFF !important; }}
                    .text-dark {{ color: #E2E8F0 !important; }}
                    .label-dark {{ color: #94A3B8 !important; }}
                    .price-gold {{ color: #FBBF24 !important; }}
                }}

                @media only screen and (max-width: 620px) {{
                    .email-container {{ width: 100% !important; border-radius: 0 !important; }}
                    .mobile-p-20 {{ padding: 24px 18px !important; }}
                    .mobile-col {{ display: block !important; width: 100% !important; padding-bottom: 14px !important; }}
                }}
            </style>
        </head>
        <body class='body' style='margin: 0; padding: 0; background-color: {BackgroundLight}; -webkit-text-size-adjust: 100%;'>
            <!-- Hidden Preheader for Mail Inboxes -->
            <div style='display: none; font-size: 1px; color: #fff; line-height: 1px; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;'>
                {preheader}
            </div>

            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' class='email-wrapper' style='background-color: {BackgroundLight}; width: 100%; padding: 36px 0;'>
                <tr>
                    <td align='center'>
                        <!-- START GMAIL BLEND HACK WRAPPER -->
                        <div class='gmail-blend-screen'>
                            <div class='gmail-blend-difference'>
                                <!-- Main Luxury Card Container with 2px Solid Gold Frame -->
                                <table role='presentation' class='email-container white-card' width='600' cellspacing='0' cellpadding='0' border='0' style='width: 600px; max-width: 600px; background-color: #FFFFFF; border-radius: 20px; overflow: hidden; border: 2px solid {BorderGold}; box-shadow: 0 14px 40px rgba(6,45,77,0.1);'>
                            
                            <!-- Luxury Header with Gold Crown Bar -->
                            <tr>
                                <td class='white-card' style='background-color: #FFFFFF; padding: 42px 30px; text-align: center; border-bottom: 2px solid {BorderGold};'>
                                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                        <tr>
                                            <td align='center'>
                                                <!-- Emblem -->
                                                <img src='{LogoUrl}' alt='Seadora Emblem' width='62' height='62' style='display: block; margin: 0 auto 16px auto; width: 62px; height: 62px;' />

                                                <!-- Luxury Brand Typography -->
                                                <h1 class='brand-title' style='margin: 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 28px; font-weight: 800; color: {BrandNavy}; letter-spacing: 4.5px; text-transform: uppercase;'>
                                                    SEADORA TRAVEL
                                                </h1>
                                                <div style='margin: 6px auto 8px auto; color: {GoldDark}; font-size: 12px; letter-spacing: 5px; font-weight: 800;'>
                                                    ✦ • ✦
                                                </div>
                                                <p style='margin: 0; font-size: 11.5px; font-weight: 800; color: {GoldDark}; letter-spacing: 2px; text-transform: uppercase;'>
                                                    {s.HeaderSubtitle}
                                                </p>

                                                <!-- Status Badge Pill -->
                                                <div style='margin-top: 20px;'>
                                                    <span style='display: inline-block; padding: 7px 24px; background-color: {badgeBg}; border: 2px solid {badgeBorder}; border-radius: 30px; font-size: 11.5px; font-weight: 800; color: {badgeColor}; letter-spacing: 1.5px; text-transform: uppercase;'>
                                                        {statusPill}
                                                    </span>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>";
    }

    private static string GetEmailFooter(EmailStrings s)
    {
        return $@"
                            <!-- Concierge Direct Action Bar -->
                            <tr>
                                <td class='white-card' style='background-color: {CardWhite}; padding: 30px 34px; border-top: 2px solid {BorderStrong}; text-align: center;'>
                                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                        <tr>
                                            <td align='center'>
                                                <p class='heading-dark' style='margin: 0 0 8px 0; font-size: 14.5px; font-weight: 800; color: {BrandNavy}; text-transform: uppercase; letter-spacing: 1px;'>
                                                    {s.ConciergeTitle}
                                                </p>
                                                <p class='text-dark' style='margin: 0 0 18px 0; font-size: 14px; color: {TextBody}; line-height: 1.5;'>
                                                    {s.ConciergeDesc}
                                                </p>
                                                <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center'>
                                                    <tr>
                                                        <td style='border-radius: 12px; background-color: #16A34A; text-align: center; box-shadow: 0 6px 18px rgba(22,163,74,0.3);'>
                                                            <a href='{WhatsAppUrl}' target='_blank' style='display: inline-block; padding: 14px 32px; font-size: 13.5px; font-weight: 800; color: #ffffff !important; letter-spacing: 0.5px;'>
                                                                <span style='color: #ffffff !important;'>{s.WhatsAppCta}</span>
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <!-- High-Contrast Luxury Footer -->
                            <tr>
                                <td class='card-highlight' style='background-color: {CardSubtle}; padding: 34px 24px; text-align: center; font-size: 12px; line-height: 1.6; border-top: 2px solid {BorderStrong};'>
                                    <p class='brand-title' style='margin: 0 0 6px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 16px; font-weight: 800; color: {BrandNavy}; letter-spacing: 2px;'>
                                        SEADORA LUXURY TRAVEL
                                    </p>
                                    <p class='label-dark' style='margin: 0 0 10px 0; font-size: 12px; color: {TextLabel}; font-weight: 700;'>
                                        {s.FooterMarina} • Tel: {ContactChannels.WhatsAppNumber}
                                    </p>
                                    <p class='label-dark' style='margin: 0 0 16px 0; font-size: 12px; color: {TextLabel};'>
                                        Inquiries: <a href='mailto:{ContactChannels.InfoEmail}' style='color: {BrandNavy}; font-weight: 800; text-decoration: underline;'>{ContactChannels.InfoEmail}</a> • Website: <a href='{WebsiteUrl}' style='color: {BrandNavy}; font-weight: 800; text-decoration: underline;'>{WebsiteUrl}</a>
                                    </p>
                                    <div style='border-top: 1px solid {BorderStrong}; padding-top: 16px; font-size: 11.5px; color: {TextLabel}; font-weight: 600;'>
                                        &copy; {DateTime.UtcNow.Year} Seadora Travel. {s.FooterRights}
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <!-- END GMAIL BLEND HACK WRAPPER -->
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
    }

    public static string BuildReceiptHtml(Domain.Entities.Booking booking)
    {
        var s = GetStrings(booking.Language);
        var refCode = booking.Id.ToString().Substring(0, 8).ToUpper();
        var tourDateFormatted = booking.TourDate.HasValue ? booking.TourDate.Value.ToString("dddd, MMMM dd, yyyy") : s.DateTbd;
        var pickupInfo = string.IsNullOrWhiteSpace(booking.PickupTime) ? s.PickupTbd : booking.PickupTime;
        var hotelInfo = string.IsNullOrWhiteSpace(booking.HotelName) ? "Hotel Pickup" : $"{booking.HotelName} {(string.IsNullOrWhiteSpace(booking.RoomNumber) ? "" : $"(Room {booking.RoomNumber})")}";

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            s: s,
            preheader: s.PreheaderReceived,
            statusPill: s.StatusReceived,
            title: s.TitleReceived,
            isConfirmed: false
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20 white-card' style='padding: 38px 36px 28px 36px; background-color: #FFFFFF;'>
                <p class='price-gold' style='margin: 0 0 6px 0; font-size: 12px; font-weight: 800; color: {GoldAmber}; text-transform: uppercase; letter-spacing: 1.5px;'>
                    ★ VIP Hospitality
                </p>
                <h2 class='heading-dark' style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 800; color: {BrandNavy}; line-height: 1.3;'>
                    {s.Greeting} {booking.CustomerName},
                </h2>
                <p class='text-dark' style='margin: 0 0 28px 0; font-size: 15px; font-weight: 500; color: {TextCharcoal}; line-height: 1.7;'>
                    {s.ReceivedIntro}
                </p>

                <!-- Boarding Pass Style Ticket Card with 2px Solid Gold Frame -->
                <table role='presentation' class='card-subtle' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: {CardSubtle}; border: 2px solid {BorderGold}; border-radius: 16px; overflow: hidden; margin-bottom: 28px; box-shadow: 0 6px 20px rgba(6,45,77,0.08);'>
                    <tr>
                        <td class='white-card' style='background-color: #FFFFFF; padding: 18px 24px; border-bottom: 2px solid {BorderGold};'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td>
                                        <div class='label-dark' style='font-size: 10.5px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 1.5px;'>
                                            {s.VoucherRef}
                                        </div>
                                        <div class='brand-title' style='font-family: ""Playfair Display"", serif; font-size: 22px; font-weight: 800; color: {BrandNavy}; letter-spacing: 2px;'>
                                            #{refCode}
                                        </div>
                                    </td>
                                    <td align='right'>
                                        <span style='display: inline-block; padding: 6px 16px; background-color: {AmberBg}; border: 1.5px solid {AmberBorder}; border-radius: 20px; font-size: 11.5px; font-weight: 800; color: {AmberText};'>
                                            {(booking.TripType ?? s.DefaultExperience)}
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 24px 22px;'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td class='mobile-col' width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div class='label-dark' style='font-size: 11px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>{s.TourDate}</div>
                                        <div class='value-dark' style='font-size: 15.5px; font-weight: 800; color: {BrandNavy}; margin-top: 4px;'>{tourDateFormatted}</div>
                                    </td>
                                    <td class='mobile-col' width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div class='label-dark' style='font-size: 11px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>{s.PickupWindow}</div>
                                        <div class='value-dark' style='font-size: 15.5px; font-weight: 800; color: {BrandNavy}; margin-top: 4px;'>{pickupInfo}</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='mobile-col' width='50%' valign='top'>
                                        <div class='label-dark' style='font-size: 11px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>{s.Guests}</div>
                                        <div class='value-dark' style='font-size: 15px; font-weight: 800; color: {BrandNavy}; margin-top: 4px;'>{booking.Guests}</div>
                                    </td>
                                    <td class='mobile-col' width='50%' valign='top'>
                                        <div class='label-dark' style='font-size: 11px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>{s.TotalAmount}</div>
                                        <div class='price-gold' style='font-size: 21px; font-weight: 800; color: {GoldAmber}; margin-top: 4px;'>${booking.TotalPrice:N2}</div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class='card-highlight' style='background-color: {CardSubtle}; padding: 16px 22px; border-top: 1.5px solid {BorderStrong}; font-size: 13px; color: {TextCharcoal}; font-weight: 600;'>
                            📍 <strong class='value-dark' style='color: {BrandNavy};'>{s.PickupLocation}:</strong> {hotelInfo}
                        </td>
                    </tr>
                </table>");

        // Optional Add-ons section
        if (booking.SelectedAddons != null && booking.SelectedAddons.Count > 0)
        {
            sb.Append($@"
                <div class='card-highlight' style='margin-bottom: 26px; padding: 18px 22px; background-color: {CardSubtle}; border-radius: 14px; border: 1.5px solid {BorderStrong};'>
                    <div class='heading-dark' style='font-size: 12px; font-weight: 800; color: {BrandNavy}; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px;'>
                        {s.SelectedAddonsLabel}
                    </div>
                    <ul class='text-dark' style='margin: 0; padding-left: 18px; font-size: 13.5px; color: {TextCharcoal}; line-height: 1.6;'>");
            foreach (var addon in booking.SelectedAddons)
            {
                sb.Append($"<li style='margin-bottom: 4px;'>{addon.Title} — <strong class='price-gold' style='color: {GoldAmber};'>${addon.TotalPrice:N2}</strong></li>");
            }
            sb.Append("</ul></div>");
        }

        sb.Append($@"
                <!-- Next Steps Info Box -->
                <div class='card-highlight' style='background-color: {CardSubtle}; border-left: 4px solid {BrandNavy}; padding: 20px 22px; border-radius: 8px; margin-bottom: 24px; border-top: 1px solid {BorderStrong}; border-right: 1px solid {BorderStrong}; border-bottom: 1px solid {BorderStrong};'>
                    <h4 class='heading-dark' style='margin: 0 0 6px 0; font-size: 13.5px; font-weight: 800; color: {BrandNavy}; text-transform: uppercase; letter-spacing: 0.5px;'>
                        {s.WhatNextTitle}
                    </h4>
                    <p class='text-dark' style='margin: 0; font-size: 13.5px; color: {TextCharcoal}; line-height: 1.65;'>
                        {s.WhatNextDesc}
                    </p>
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter(s));
        return sb.ToString();
    }

    public static string BuildConfirmationHtml(Domain.Entities.Booking booking)
    {
        var s = GetStrings(booking.Language);
        var refCode = booking.Id.ToString().Substring(0, 8).ToUpper();
        var tourDateFormatted = booking.TourDate.HasValue ? booking.TourDate.Value.ToString("dddd, MMMM dd, yyyy") : s.DateTbd;
        var pickupInfo = string.IsNullOrWhiteSpace(booking.PickupTime) ? "09:00 AM (Sharp)" : booking.PickupTime;
        var hotelInfo = string.IsNullOrWhiteSpace(booking.HotelName) ? "Private Luxury Vehicle Transfer" : $"{booking.HotelName} {(string.IsNullOrWhiteSpace(booking.RoomNumber) ? "" : $"(Room {booking.RoomNumber})")}";

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            s: s,
            preheader: s.PreheaderConfirmed,
            statusPill: s.StatusConfirmed,
            title: s.TitleConfirmed,
            isConfirmed: true
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20 white-card' style='padding: 38px 36px 28px 36px; background-color: #FFFFFF;'>
                <p style='margin: 0 0 6px 0; font-size: 12px; font-weight: 800; color: {GreenBorder}; text-transform: uppercase; letter-spacing: 1.5px;'>
                    ✓ {s.StatusConfirmed}
                </p>
                <h2 class='heading-dark' style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 800; color: {BrandNavy}; line-height: 1.3;'>
                    {s.Greeting} {booking.CustomerName},
                </h2>
                <p class='text-dark' style='margin: 0 0 28px 0; font-size: 15px; font-weight: 500; color: {TextCharcoal}; line-height: 1.7;'>
                    {s.ConfirmedIntro}
                </p>

                <!-- Boarding Pass Style Voucher Card with 2px Solid Gold Frame -->
                <table role='presentation' class='card-subtle' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: {CardSubtle}; border: 2px solid {BorderGold}; border-radius: 16px; overflow: hidden; margin-bottom: 28px; box-shadow: 0 8px 24px rgba(6,45,77,0.08);'>
                    <tr>
                        <td class='white-card' style='background-color: #FFFFFF; padding: 20px 24px; border-bottom: 2px solid {GreenBorder};'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td>
                                        <div class='label-dark' style='font-size: 10.5px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 1.5px;'>
                                            {s.OfficialVoucher}
                                        </div>
                                        <div class='brand-title' style='font-family: ""Playfair Display"", serif; font-size: 22px; font-weight: 800; color: {BrandNavy}; letter-spacing: 2px;'>
                                            #{refCode}
                                        </div>
                                    </td>
                                    <td align='right'>
                                        <div style='display: inline-block; padding: 6px 18px; background-color: {GreenBg}; border: 2px solid {GreenBorder}; border-radius: 20px; font-size: 11.5px; font-weight: 800; color: {GreenEmerald}; letter-spacing: 1px; text-transform: uppercase;'>
                                            {s.StatusConfirmed}
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 24px 22px;'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td class='mobile-col' width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div class='label-dark' style='font-size: 11px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>{s.TourDate}</div>
                                        <div class='value-dark' style='font-size: 15.5px; font-weight: 800; color: {BrandNavy}; margin-top: 4px;'>{tourDateFormatted}</div>
                                    </td>
                                    <td class='mobile-col' width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div class='label-dark' style='font-size: 11px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>{s.PickupWindow}</div>
                                        <div class='value-dark' style='font-size: 15.5px; font-weight: 800; color: {BrandNavy}; margin-top: 4px;'>{pickupInfo}</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='mobile-col' width='50%' valign='top'>
                                        <div class='label-dark' style='font-size: 11px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>{s.Guests}</div>
                                        <div class='value-dark' style='font-size: 15px; font-weight: 800; color: {BrandNavy}; margin-top: 4px;'>{booking.Guests}</div>
                                    </td>
                                    <td class='mobile-col' width='50%' valign='top'>
                                        <div class='label-dark' style='font-size: 11px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>{s.TotalAmount}</div>
                                        <div class='price-gold' style='font-size: 21px; font-weight: 800; color: {GoldAmber}; margin-top: 4px;'>${booking.TotalPrice:N2}</div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class='card-highlight' style='background-color: {CardSubtle}; padding: 16px 22px; border-top: 1.5px solid {BorderStrong}; font-size: 13px; color: {TextCharcoal}; font-weight: 600;'>
                            📍 <strong class='value-dark' style='color: {BrandNavy};'>{s.PickupLocation}:</strong> {hotelInfo}
                        </td>
                    </tr>
                </table>

                <!-- Registered Guests list if present -->
                {(booking.GuestsList != null && booking.GuestsList.Count > 0 ? $@"
                <div class='card-highlight' style='margin-bottom: 26px; padding: 18px 22px; background-color: {CardSubtle}; border-radius: 14px; border: 1.5px solid {BorderStrong};'>
                    <div class='heading-dark' style='font-size: 12px; font-weight: 800; color: {BrandNavy}; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px;'>
                        {s.GuestsBreakdownLabel}
                    </div>
                    <ul class='text-dark' style='margin: 0; padding-left: 18px; font-size: 13.5px; color: {TextCharcoal}; line-height: 1.6;'>
                        {string.Join("", booking.GuestsList.ConvertAll(g => $"<li style='margin-bottom: 4px;'>{g.FullName} ({g.Nationality ?? "VIP"})</li>"))}
                    </ul>
                </div>" : "")}

                <!-- VIP Travel Guidelines Box -->
                <div class='card-highlight' style='background-color: {CardSubtle}; border-left: 4px solid {BrandNavy}; padding: 20px 22px; border-radius: 8px; margin-bottom: 24px; border-top: 1px solid {BorderStrong}; border-right: 1px solid {BorderStrong}; border-bottom: 1px solid {BorderStrong};'>
                    <h4 class='heading-dark' style='margin: 0 0 8px 0; font-size: 13.5px; font-weight: 800; color: {BrandNavy}; text-transform: uppercase; letter-spacing: 0.5px;'>
                        {s.RemindersTitle}
                    </h4>
                    <ul class='text-dark' style='margin: 0; padding-left: 18px; font-size: 13.5px; color: {TextCharcoal}; line-height: 1.65;'>
                        <li style='margin-bottom: 6px;'>{s.Reminder1}</li>
                        <li style='margin-bottom: 6px;'>{s.Reminder2}</li>
                        <li>{s.Reminder3}</li>
                    </ul>
                </div>

                <!-- Cancellation Guarantee -->
                <div style='background-color: {AmberBg}; border: 1.5px solid {AmberBorder}; padding: 16px 20px; border-radius: 8px; font-size: 13px; color: {AmberText}; line-height: 1.55; font-weight: 600;'>
                    <strong style='color: {AmberText};'>{s.FlexibilityTitle}:</strong> {s.FlexibilityDesc}
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter(s));
        return sb.ToString();
    }

    public static string BuildInquiryAutoReplyHtml(ContactInquiry inquiry)
    {
        var s = GetStrings("en");
        var destination = string.IsNullOrWhiteSpace(inquiry.DestinationInterest) ? "Egypt & The Red Sea" : inquiry.DestinationInterest;

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            s: s,
            preheader: $"Thank you for contacting Seadora Travel regarding {destination}.",
            statusPill: "Inquiry Received",
            title: "Thank You for Contacting Seadora Travel",
            isConfirmed: false
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20 white-card' style='padding: 38px 36px 28px 36px; background-color: #FFFFFF;'>
                <p class='price-gold' style='margin: 0 0 6px 0; font-size: 12px; font-weight: 800; color: {GoldAmber}; text-transform: uppercase; letter-spacing: 1.5px;'>
                    Inquiry Acknowledgment
                </p>
                <h2 class='heading-dark' style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 800; color: {BrandNavy}; line-height: 1.3;'>
                    Dear {inquiry.FullName},
                </h2>
                <p class='text-dark' style='margin: 0 0 20px 0; font-size: 15px; color: {TextCharcoal}; line-height: 1.7;'>
                    Thank you for reaching out to <strong>Seadora Luxury Travel</strong>. We have received your inquiry regarding <strong>{destination}</strong>.
                </p>

                <!-- Inquiry Summary Box -->
                <table role='presentation' class='card-highlight' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: {CardSubtle}; border: 1.5px solid {BorderStrong}; border-radius: 12px; padding: 20px; margin-bottom: 24px;'>
                    <tr>
                        <td>
                            <div class='heading-dark' style='font-size: 12px; font-weight: 800; color: {BrandNavy}; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px;'>Your Message / Request:</div>
                            <div class='text-dark' style='font-size: 13.5px; color: {TextCharcoal}; font-style: italic; line-height: 1.6;'>
                                ""{inquiry.Message}""
                            </div>
                            {(string.IsNullOrWhiteSpace(inquiry.DateOrGuests) ? "" : $"<div class='label-dark' style='margin-top: 10px; font-size: 12.5px; color: {TextLabel}; font-weight: 700;'><strong>Preferred Timing / Group:</strong> {inquiry.DateOrGuests}</div>")}
                        </td>
                    </tr>
                </table>

                <p class='text-dark' style='margin: 0 0 20px 0; font-size: 15px; color: {TextCharcoal}; line-height: 1.7;'>
                    One of our senior destination designers is preparing a personalized recommendation tailored to your schedule. We typically reply within 2 to 4 business hours.
                </p>
            </td>
        </tr>");

        sb.Append(GetEmailFooter(s));
        return sb.ToString();
    }

    public static string BuildAdminInquiryReplyHtml(ContactInquiry inquiry, string replyMessage)
    {
        var s = GetStrings("en");
        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            s: s,
            preheader: $"Seadora Travel has replied to your travel inquiry.",
            statusPill: "Concierge Response",
            title: "Response to your Inquiry — Seadora Travel",
            isConfirmed: false
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20 white-card' style='padding: 38px 36px 28px 36px; background-color: #FFFFFF;'>
                <p class='price-gold' style='margin: 0 0 6px 0; font-size: 12px; font-weight: 800; color: {GoldAmber}; text-transform: uppercase; letter-spacing: 1.5px;'>
                    Seadora Concierge Team
                </p>
                <h2 class='heading-dark' style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 800; color: {BrandNavy}; line-height: 1.3;'>
                    Dear {inquiry.FullName},
                </h2>
                <div class='text-dark card-highlight' style='margin: 0 0 24px 0; font-size: 14.5px; color: {TextCharcoal}; line-height: 1.7; background-color: {CardSubtle}; border-left: 4px solid {BrandNavy}; padding: 22px; border-radius: 8px; border-top: 1px solid {BorderStrong}; border-right: 1px solid {BorderStrong}; border-bottom: 1px solid {BorderStrong};'>
                    {replyMessage.Replace("\n", "<br/>")}
                </div>

                <!-- Original Inquiry Reference -->
                <div style='border-top: 1.5px solid {BorderStrong}; padding-top: 18px; margin-bottom: 20px;'>
                    <span class='label-dark' style='font-size: 11.5px; font-weight: 800; color: {TextLabel}; text-transform: uppercase; letter-spacing: 0.5px;'>Regarding your original message:</span>
                    <p class='label-dark' style='margin: 6px 0 0 0; font-size: 13px; font-weight: 600; color: {TextLabel}; font-style: italic;'>
                        ""{inquiry.Message}""
                    </p>
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter(s));
        return sb.ToString();
    }
}
